using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using Cinemachine;
public class MonopolyGo : MonoBehaviourPunCallbacks
{
    public GameObject playerIcon; // The icon that moves
    public Button goButton; // The button to start the move
    public Transform[] tiles; // Array of tile positions
    public float moveDuration = 0.5f; // Time taken to move to the next tile
    public float jumpHeight = 1.0f; // Height of the jump
    public int jumpCount = 1; // Number of jumps to perform

    public Transform dice1; // Transform of the first dice
    public Transform dice2; // Transform of the second dice
    public float diceRollDuration = 1f; // Duration for the dice roll animation

    public int currentTileIndex = 0; // Player's current tile index
    private int currentTurn = 0; // Current player's turn
    private int playerIndex; // Local player's index
    public bool isMultiplayer; // Check if the game is multiplayer

    public PlayerClass playerClass; // Enum to track the player's class
    public bool currentPlayerIsInJail = false;
    public float diceRollToRelease = 0;

    public GameObject dice;
    int die1;
    int die2;

    public PlayerIconStatus[] playerIconStatus;
    public CinemachineVirtualCamera virtualCamera;
    public static MonopolyGo instance; // Singleton instance

    void Awake()
    {
        if (!instance)
            instance = this;
    }

    public void Start()
    {
        isMultiplayer = PhotonNetwork.IsConnected;
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            isMultiplayer = false;
            UIManager.instance.LeaveButtonMultiplayer.SetActive(false);

        }
        if (isMultiplayer)
        {
            UIManager.instance.LeaveButtonMultiplayer.SetActive(true);
            // Assign random player class
            playerClass = (PlayerClass)Random.Range(0, System.Enum.GetValues(typeof(PlayerClass)).Length);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerClass", playerClass } });

            if (PhotonNetwork.IsMasterClient)
            {
                //photonView.RPC(nameof(OnClickLetStart), RpcTarget.AllBuffered);
                InitializePlayers();
                // Initialize turn management
                if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("CurrentTurn"))
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CurrentTurn", 0 } });
                }
            }

            playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        }
        else
        {
            playerClass = PlayerClass.HigherEarner; // Default for single-player
            UIManager.instance.PlayerGameInfoScrollView.SetActive(false);
        }

        goButton.onClick.AddListener(OnGoButtonClicked);
        UpdateGoButtonState();
    }

    public void OnGoButtonClicked()
    {
        if (isMultiplayer)
        {
            photonView.RPC(nameof(MovePlayerRPC), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            StartCoroutine(MovePlayer());
        }
        goButton.interactable = false;
    }

    IEnumerator MovePlayer()
    {

        AssignPlayerIcon();
        dice.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        int diceSum = RollDice(); // Roll the dice
        yield return new WaitForSeconds(1.3f); // Animate dice rolling
        dice.SetActive(false);


        for (int i = 0; i < diceSum; i++)
        {
            currentTileIndex = (currentTileIndex + 1) % tiles.Length;
            UpdateTileIndex(currentTileIndex);
            if (AiMatchFinding.instance.AiMatchIsPlaying)
            {
                playerIconStatus[AiMatchFinding.instance.turnOfPlayer].currentTileIndex = currentTileIndex;
            }
            SettingManager.instance.IconMoveSound();
            // Sync the new tile index and hat position across the network
            // Sync the hat movement
            //photonView.RPC(nameof(SyncHatPosition), RpcTarget.AllBuffered, tiles[currentTileIndex].position);
            if (isMultiplayer)
                photonView.RPC(nameof(SyncMoveToPosition), RpcTarget.AllBuffered, tiles[currentTileIndex].position);
            yield return MoveToPosition(tiles[currentTileIndex].position);

            if (i == diceSum - 1)
            {
                ActivateStepPanel(currentTileIndex); // Activate step panel
            }
            else
            {
                CheckTilePrize(currentTileIndex); // Handle passing tile prizes
            }
        }




    }

    public void ActiveAllIcons()
    {
        for (int i = 0; i < playerIconStatus.Length; i++)
        {
            playerIconStatus[i].icon.SetActive(true);
        }
    }
    void AssignPlayerIcon()
    {
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            currentTileIndex = playerIconStatus[AiMatchFinding.instance.turnOfPlayer].currentTileIndex;
            print("Current Tile Index By AI Mode: "+ currentTileIndex);
        }
        if (!AiMatchFinding.instance.AiMatchIsPlaying)
        {
            return;
        }
        switch (AiMatchFinding.instance.turnOfPlayer)
        {
            case 0:
                playerIcon = playerIconStatus[0].icon ;
                break;
            case 1:
                playerIcon = playerIconStatus[1].icon;
                break;
            case 2:
                playerIcon = playerIconStatus[2].icon;
                break;
            case 3:
                playerIcon = playerIconStatus[3].icon;
                break;

        }
        virtualCamera.m_Follow = playerIcon.transform;

    }

    public void EndTurn()
    {
        Debug.Log("Turn Incremented");
        if (isMultiplayer)
        {
            print("End Turn");
            photonView.RPC(nameof(EndTurnRPC), RpcTarget.AllBuffered);
        }
        else
        {
            AiMatchFinding.instance.TurnIncremented();
        }
    }
    [PunRPC]
    void SyncHatPosition(Vector3 newPosition)
    {
        // Move the hat to the new position on all clients
        this.transform.position = newPosition;
    }
    IEnumerator StartGameWithDelay()
    {
        yield return new WaitForSeconds(1); // Allow time for all players to initialize
        photonView.RPC(nameof(OnClickLetStart), RpcTarget.AllBuffered);
    }
    [PunRPC]
    void MovePlayerRPC(int playerId)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            StartCoroutine(MovePlayer());
        }
    }

    [PunRPC]
    void EndTurnRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentTurn"];
            CheckForEliminations();
            //AddCashToCurrentTurnPlayer(200);
            currentTurn = (currentTurn + 1) % PhotonNetwork.CurrentRoom.PlayerCount;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CurrentTurn", currentTurn } });


        }
    }

    void UpdateGoButtonState()
    {
        if (isMultiplayer)
        {
            print("Current Room: " + PhotonNetwork.CurrentRoom);
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CurrentTurn", out object turnObj))
            {
                int turn = (int)turnObj;
                goButton.interactable = (PhotonNetwork.LocalPlayer.ActorNumber - 1 == turn);
                SyncPlayerTurnVisual(turn);
            }
        }
        else
        {
            goButton.interactable = true;
        }
    }
    void UpdateTileIndex(int newTileIndex)
    {
        // Update the local currentTileIndex
        currentTileIndex = newTileIndex;

        // Update the room property
        if (isMultiplayer)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CurrentTileIndex", currentTileIndex } });

        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("CurrentTurn"))
        {
            UpdateGoButtonState();
        }
        if (propertiesThatChanged.ContainsKey("CurrentTileIndex"))
        {
            currentTileIndex = (int)propertiesThatChanged["CurrentTileIndex"];
            // You may want to perform additional actions here, like moving the player icon
        }
    }

    int RollDice()
    {
        die1 = Random.Range(1, 7);
        die2 = Random.Range(1, 7);
        SettingManager.instance.DiceSound();
        switch (die1)
        {
            case 1:
                dice1.DOLocalRotate(new Vector3(0, 0, 180), 0.5f, RotateMode.Fast);
                print("Case 1");
                break;
            case 2:
                dice1.DOLocalRotate(new Vector3(0, 0, 0), 0.5f, RotateMode.Fast);
                print("Case 2");

                break;
            case 3:
                dice1.DOLocalRotate(new Vector3(0, 0, -90), 0.5f, RotateMode.Fast);
                print("Case 3");

                break;
            case 5:
                dice1.DOLocalRotate(new Vector3(0, 0, -270), 0.5f, RotateMode.Fast);
                print("Case 5");

                break;
            case 4:
                dice1.DOLocalRotate(new Vector3(90, 0, 0), 0.5f, RotateMode.Fast);
                print("Case 4");

                break;
            case 6:
                dice1.DOLocalRotate(new Vector3(270, 0, 0), 0.5f, RotateMode.Fast);
                print("Case 6");

                break;
        }

        switch (die2)
        {
            case 1:
                dice2.DOLocalRotate(new Vector3(0, 0, 180), 0.5f, RotateMode.Fast);
                print("Case 1");
                break;
            case 2:
                dice2.DOLocalRotate(new Vector3(0, 0, 0), 0.5f, RotateMode.Fast);
                print("Case 2");

                break;
            case 3:
                dice2.DOLocalRotate(new Vector3(0, 0, -90), 0.5f, RotateMode.Fast);
                print("Case 3");

                break;
            case 5:
                dice2.DOLocalRotate(new Vector3(0, 0, -270), 0.5f, RotateMode.Fast);
                print("Case 5");

                break;
            case 4:
                dice2.DOLocalRotate(new Vector3(90, 0, 0), 0.5f, RotateMode.Fast);
                print("Case 4");

                break;
            case 6:
                dice2.DOLocalRotate(new Vector3(270, 0, 0), 0.5f, RotateMode.Fast);
                print("Case 6");

                break;
        }
        return die1 + die2;
    }


    IEnumerator AnimateDiceRoll(int targetSum)
    {
        float timePassed = 0;
        while (timePassed < diceRollDuration)
        {
            dice1.Rotate(Random.Range(90f, 180f) * Time.deltaTime, 0, 0);
            dice2.Rotate(Random.Range(90f, 180f) * Time.deltaTime, 0, 0);
            timePassed += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"Dice Rolled: {targetSum}");
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        playerIcon.transform.DOJump(targetPosition, jumpHeight, jumpCount, moveDuration).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(moveDuration);

    }
    [PunRPC]
    void SyncMoveToPosition(Vector3 targetPosition)
    {
        // Ensure the animation runs with the same parameters across all clients
        playerIcon.transform.DOJump(targetPosition, jumpHeight, jumpCount, moveDuration)
            .SetEase(Ease.OutQuad); // The same animation as on the local client
    }
    void ActivateStepPanel(int tileIndex)
    {
        TileManager.instance.GiveTileReward(tiles[tileIndex].GetComponent<TileInfo>());
        //Debug.Log($"Landed on tile: {tileIndex}");
    }

    void CheckTilePrize(int tileIndex)
    {
        //Debug.Log($"Passed tile: {tileIndex}");
        if (tiles[tileIndex].GetComponent<TileInfo>().tileName == GlobalData.TileName.Go)
        {
            UIManager.instance.UpdateMoneyInMatch(tiles[tileIndex].GetComponent<TileInfo>().money);
        }
    }

    [PunRPC]
    void OnClickLetStart()
    {
        UIManager.instance.RoomPanel.SetActive(false);
        UIManager.instance.BottomPanel.SetActive(true);
        UIManager.instance.PlayerGameInfoScrollView.SetActive(true);
    }

    //public void OnStartGame()
    //{
    //    OnClickLetStart();
    //    //photonView.RPC(nameof(OnClickLetStart), RpcTarget.AllBuffered);
    //}
    #region All Player Data
    public enum PlayerClass
    {
        HigherEarner,
        MiddleEarner,
        EveryDayEarner,
        EmergingEarner
    }

    [System.Serializable]
    public class PlayerData
    {
        public int playerId;
        public string playerName;
        public PlayerClass playerClass;
        public int cash;

        public PlayerData(int id, string name, PlayerClass pClass, int startingCash)
        {
            playerId = id;
            playerName = name;
            playerClass = pClass;
            cash = startingCash;
        }
    }
    public List<PlayerData> playerDataList = new List<PlayerData>(); // List to store all player data
    [PunRPC]
    public void InitializePlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerDataList.Clear();

            foreach (var player in PhotonNetwork.PlayerList)
            {
                string playerName = player.NickName;
                PlayerClass playerClass = (PlayerClass)Random.Range(0, System.Enum.GetValues(typeof(PlayerClass)).Length);
                int startingCash = GetStartingCash(playerClass);

                PlayerData newPlayerData = new PlayerData(player.ActorNumber, playerName, playerClass, startingCash);
                playerDataList.Add(newPlayerData);
            }

            SyncPlayerData();
            DG.Tweening.DOVirtual.DelayedCall(1,()=> SyncPlayerTurnVisual(1));
        }
    }

    int GetStartingCash(PlayerClass playerClass)
    {
        return playerClass switch
        {
            PlayerClass.HigherEarner => 2500,
            PlayerClass.MiddleEarner => 1500,
            PlayerClass.EveryDayEarner => 1000,
            PlayerClass.EmergingEarner => 500,
            _ => 0
        };
    }

    void SyncPlayerData()
    {
        string serializedData = SerializePlayerData();
        photonView.RPC(nameof(SyncPlayerDataRPC), RpcTarget.AllBuffered, serializedData);
    }
    [PunRPC]
    void SyncPlayerDataRPC(string serializedData)
    {
        playerDataList = DeserializePlayerData(serializedData);
        DisplayPlayerInfo();
    }
    string SerializePlayerData()
    {
        List<string> dataStrings = new List<string>();
        foreach (var data in playerDataList)
        {
            string dataString = $"{data.playerId},{data.playerName},{(int)data.playerClass},{data.cash}";
            dataStrings.Add(dataString);
        }
        return string.Join(";", dataStrings);
    }
    List<PlayerData> DeserializePlayerData(string serializedData)
    {
        List<PlayerData> dataList = new List<PlayerData>();
        string[] dataEntries = serializedData.Split(';');

        foreach (var entry in dataEntries)
        {
            string[] fields = entry.Split(',');
            int id = int.Parse(fields[0]);
            string name = fields[1];
            PlayerClass pClass = (PlayerClass)int.Parse(fields[2]);
            int cash = int.Parse(fields[3]);
            dataList.Add(new PlayerData(id, name, pClass, cash));
        }

        return dataList;
    }
    public PlayerData GetPlayerData(int playerId)
    {
        return playerDataList.Find(p => p.playerId == playerId);
    }
    public void AddCashToCurrentTurnPlayer(int cashAmount)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CurrentTurn", out object currentTurnObj))
        {
            print("Current Object: " + currentTurnObj);
            int currentTurnPlayerId = (int)currentTurnObj + 1;
            UpdatePlayerCash(currentTurnPlayerId, cashAmount);
            Debug.Log($"Added {cashAmount} cash to Player {currentTurnPlayerId}");
        }
    }

    public void UpdatePlayerCash(int playerId, int amount)
    {
        PlayerData player = GetPlayerData(playerId);
        if (player != null)
        {
            player.cash += amount;
            SyncPlayerData();
        }
    }
    
    public void SyncPlayerTurnVisual(int playerID)
    {
        photonView.RPC(nameof(UpdatePlayerTurn), RpcTarget.AllBuffered, playerID);
    }
    [PunRPC]
    public void UpdatePlayerTurn(int playerID)
    {
        ClearPlayerInfoList();
        foreach (var player in playerDataList)
        {
            
            Debug.Log($"Player {player.playerId}: {player.playerClass}, Cash: {player.cash}");
            GameObject init = Instantiate(UIManager.instance.PlayerGameInfoPrefab, UIManager.instance.PlayerGameInfoContent);
            if (player.playerId == playerID)
            {
                init.GetComponent<Image>().color = Color.green;
            }
            init.GetComponent<PlayerGameInfo>().PlayerID.text = player.playerId.ToString();
            init.GetComponent<PlayerGameInfo>().PlayerName.text = player.playerName;
            init.GetComponent<PlayerGameInfo>().PlayerClass.text = player.playerClass.ToString();
            init.GetComponent<PlayerGameInfo>().PlayerCoins.text = player.cash.ToString();
            playerInfoList.Add(init);
        }
    }
    public void DisplayPlayerInfo()
    {
        ClearPlayerInfoList();
        foreach (var player in playerDataList)
        {
            Debug.Log($"Player {player.playerId}: {player.playerClass}, Cash: {player.cash}");
            GameObject init = Instantiate(UIManager.instance.PlayerGameInfoPrefab, UIManager.instance.PlayerGameInfoContent);
            init.GetComponent<PlayerGameInfo>().PlayerID.text = player.playerId.ToString();
            init.GetComponent<PlayerGameInfo>().PlayerName.text = player.playerName;
            init.GetComponent<PlayerGameInfo>().PlayerClass.text = player.playerClass.ToString();
            init.GetComponent<PlayerGameInfo>().PlayerCoins.text = player.cash.ToString();
            playerInfoList.Add(init);
        }
    }
    List<GameObject> playerInfoList = new List<GameObject>();
    void ClearPlayerInfoList()
    {
        for (int i = 0; i < playerInfoList.Count; i++)
        {
            Destroy(playerInfoList[i]);
        }
        playerInfoList = new List<GameObject>();
    }

    #endregion

    #region Eliminations
    void EliminatePlayer(int playerId)
    {
        // Find the player to eliminate
        PlayerData playerToEliminate = GetPlayerData(playerId);

        if (playerToEliminate != null)
        {
            Debug.Log($"Player {playerToEliminate.playerName} is eliminated!");

            // Remove the player from the list
            playerDataList.Remove(playerToEliminate);

            // Sync player data to all clients
            SyncPlayerData();

            // Check for victory condition
            if (playerDataList.Count == 1)
            {
                DeclareWinner(playerDataList[0]);
            }
            else
            {
                // Update the turn order
                UpdateTurnOrderAfterElimination(playerId);
            }
        }
    }

    void CheckForEliminations()
    {
        if (playerDataList.Count == 1)
        {
            DeclareWinner(playerDataList[0]);
            return;
        }
        foreach (var player in new List<PlayerData>(playerDataList)) // Create a copy to iterate safely
        {
            if (player.cash <= 0)
            {
                EliminatePlayer(player.playerId);
            }
        }
    }

    void UpdateTurnOrderAfterElimination(int eliminatedPlayerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int currentTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentTurn"];

            // Adjust the current turn if the eliminated player was the current turn
            if (currentTurn == eliminatedPlayerId - 1)
            {
                currentTurn = (currentTurn + 1) % playerDataList.Count;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CurrentTurn", currentTurn } });
            }
        }
    }

    void DeclareWinner(PlayerData winner)
    {
        Debug.Log($"Player {winner.playerName} wins the game!");

        // Add the winner's in-game cash to their main cash
        AddToMainCash(winner.playerId, winner.cash);

        // Notify all players
        DG.Tweening.DOVirtual.DelayedCall(5, () => photonView.RPC(nameof(NotifyWinnerRPC), RpcTarget.AllBuffered, winner.playerName));
    }

    void AddToMainCash(int playerId, int cashAmount)
    {

        // Add logic to store the cash in the player's main account (e.g., persistent storage or a database)
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
        {
            UIManager.instance.UpdateMoneyGlobaly(cashAmount);
        }
        Debug.Log($"Added {cashAmount} to Player {playerId}'s main cash.");
    }

    [PunRPC]
    void NotifyWinnerRPC(string winnerName)
    {
        //UIManager.instance.ShowWinnerPanel(winnerName); // Assume you have a UIManager method for this

        UIManager.instance.winnerPanel.SetActive(true);
        DG.Tweening.DOVirtual.DelayedCall(10, () => PhotonNetwork.LeaveRoom());
        //PhotonNetwork.LeaveRoom(); // Optionally leave the room after the game ends
    }



    #endregion
}
[System.Serializable]
public class PlayerIconStatus
{
    public GameObject icon;
    public int currentTileIndex;
}
