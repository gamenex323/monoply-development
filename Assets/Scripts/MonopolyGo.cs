using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;

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

    private int currentTileIndex = 0; // Player's current tile index
    private int currentTurn = 0; // Current player's turn
    private int playerIndex; // Local player's index
    private bool isMultiplayer; // Check if the game is multiplayer

    public PlayerClass playerClass; // Enum to track the player's class
    public static MonopolyGo instance; // Singleton instance

    void Awake()
    {
        if (!instance)
            instance = this;
    }

    public void Start()
    {
        isMultiplayer = PhotonNetwork.IsConnected;

        if (isMultiplayer)
        {

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
            playerClass = PlayerClass.UpperClass; // Default for single-player
            UIManager.instance.PlayerGameInfoScrollView.SetActive(false);
        }

        goButton.onClick.AddListener(OnGoButtonClicked);
        UpdateGoButtonState();
    }

    void OnGoButtonClicked()
    {
        if (isMultiplayer)
        {
            photonView.RPC(nameof(MovePlayerRPC), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            StartCoroutine(MovePlayer());
        }
    }

    IEnumerator MovePlayer()
    {
        int diceSum = RollDice(); // Roll the dice
        yield return AnimateDiceRoll(diceSum); // Animate dice rolling

        for (int i = 0; i < diceSum; i++)
        {
            currentTileIndex = (currentTileIndex + 1) % tiles.Length;
            UpdateTileIndex(currentTileIndex);
            // Sync the new tile index and hat position across the network
            // Sync the hat movement
            //photonView.RPC(nameof(SyncHatPosition), RpcTarget.AllBuffered, tiles[currentTileIndex].position);
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

        if (isMultiplayer)
        {
            photonView.RPC(nameof(EndTurnRPC), RpcTarget.AllBuffered);
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
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CurrentTurn", out object turnObj))
            {
                int turn = (int)turnObj;
                goButton.interactable = (PhotonNetwork.LocalPlayer.ActorNumber - 1 == turn);
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
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CurrentTileIndex", currentTileIndex } });
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
        int die1 = Random.Range(1, 7);
        int die2 = Random.Range(1, 7);
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
        Debug.Log($"Landed on tile: {tileIndex}");
    }

    void CheckTilePrize(int tileIndex)
    {
        Debug.Log($"Passed tile: {tileIndex}");
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
        UpperClass,
        MiddleClass,
        WorkingClass,
        LowerClass
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
        }
    }

    int GetStartingCash(PlayerClass playerClass)
    {
        return playerClass switch
        {
            PlayerClass.UpperClass => 2500,
            PlayerClass.MiddleClass => 1500,
            PlayerClass.WorkingClass => 1000,
            PlayerClass.LowerClass => 500,
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
        UIManager.instance.winnerName.text = winner.playerName;
        // Notify all players
        DG.Tweening.DOVirtual.DelayedCall(5, ()=> photonView.RPC(nameof(NotifyWinnerRPC), RpcTarget.AllBuffered, winner.playerName));
    }

    void AddToMainCash(int playerId, int cashAmount)
    {
        UIManager.instance.winnerPanel.SetActive(true);
        // Add logic to store the cash in the player's main account (e.g., persistent storage or a database)
        Debug.Log($"Added {cashAmount} to Player {playerId}'s main cash.");
    }

    [PunRPC]
    void NotifyWinnerRPC(string winnerName)
    {
        //UIManager.instance.ShowWinnerPanel(winnerName); // Assume you have a UIManager method for this
        
        PhotonNetwork.LeaveRoom(); // Optionally leave the room after the game ends
    }

    #endregion
}
