using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager instance;
    public GameObject moneyPanel, bankHiestPanel, attackPanel, shieldPanel, bottomPanel,PlayButton, winnerPanel;
    public GameObject[] AllRewardPanels;
    public TextMeshProUGUI globalMoney;
    public TextMeshProUGUI moneyPanelText;
    public TextMeshProUGUI winnerName;
    public float durationOfMoneyEffect = 2f;
    public GameObject[] selectionPanel;
    public GameObject[] aiSelectionPanel;
    public int gameSelection;
    public int aiSelection;
    void Start()
    {
        instance = this;
        UpdateMoneyInMatch(0);
        currentMaxPlayerIndex = GlobalData.MaxPlayer;
        UpdatePlayerData();
        bottomPanel.SetActive(false);
        PlayButton.SetActive(true);
       
    }

    public void GameSelectionRight()
    {
        gameSelection++;
        if (gameSelection < selectionPanel.Length)
        {
            for (int i = 0; i < selectionPanel.Length; i++)
            {
                if (i == gameSelection)
                {
                    selectionPanel[i].SetActive(true);
                }
                else
                {
                    selectionPanel[i].SetActive(false);
                }
            }
        }
    }
    public void GameSelectionLeft()
    {
        gameSelection--;
        if(gameSelection >= 0)
        {
            print("Selection is: " + gameSelection);
            for (int i = 0; i < selectionPanel.Length; i++)
            {
                if (i == gameSelection)
                {
                    selectionPanel[i].SetActive(true);
                }
                else
                {
                    selectionPanel[i].SetActive(false);
                }
            }
        }
    }

    public void AISelectionRight()
    {
        aiSelection++;
        if (aiSelection < aiSelectionPanel.Length)
        {
            print("AiSeclection: " + aiSelection);
            for (int i = 0; i < aiSelectionPanel.Length; i++)
            {
                if (i == aiSelection)
                {
                    Debug.Log("Ai Selection panel", aiSelectionPanel[i]);
                    aiSelectionPanel[i].SetActive(true);
                }
                else
                {
                    aiSelectionPanel[i].SetActive(false);
                }
            }
        }
    }
    public void AISelectionLeft()
    {
        aiSelection--;
        if (aiSelection >= 0)
        {
            for (int i = 0; i < aiSelectionPanel.Length; i++)
            {
                if (i == aiSelection)
                {
                    aiSelectionPanel[i].SetActive(true);
                }
                else
                {
                    aiSelectionPanel[i].SetActive(false);
                }
            }
        }
    }
    public void DisableAllRewardedPanels()
    {
        foreach (GameObject panel in AllRewardPanels)
        {
            panel.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void GetLeaderBoard()
    {
        PlayfabLogin.login.GetLeaderboard();
    }
    public void UpdateMoneyGlobaly(int money)
    {
        int currentMoney = GlobalData.GetMoney();
        GlobalData.SetMoney(money);
        int targetMoney = GlobalData.GetMoney();
        print("Money Update Complete");
        StartCoroutine(TypewriterEffect(globalMoney, currentMoney, targetMoney, durationOfMoneyEffect));
    }
    public void UpdateMoneyInMatch(int money)
    {
        if (MonopolyGo.instance.isMultiplayer && !AiMatchFinding.instance.AiMatchIsPlaying)
        {
            if (MonopolyGo.instance)
                MonopolyGo.instance.AddCashToCurrentTurnPlayer(money);
        }
        else if (AiMatchFinding.instance.AiMatchIsPlaying){
            AiMatchFinding.instance.UpdatePlayerProfile(money);
        }


    }

    private IEnumerator TypewriterEffect(TextMeshProUGUI textElement, float currentMoney, float targetMoney, float duration)
    {
        print("Money Update Complete");
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration); // Ensure progress stays between 0 and 1
            float updatedMoney = Mathf.Lerp(currentMoney, targetMoney, progress);

            textElement.text = Mathf.RoundToInt(updatedMoney).ToString();
            yield return null;
        }

        // Ensure the final value is set to targetMoney
        textElement.text = Mathf.RoundToInt(targetMoney).ToString();
        print("Money Update Complete");
    }
    [SerializeField] public GameObject ScoreCardPrefab;
    [SerializeField] public Transform Content;
    [SerializeField] public List<GameObject> LeaderboardCards = new List<GameObject>();
    public void DeleteLeaderBoard()
    {
        for (int i = 0; i < LeaderboardCards.Count; i++)
        {
            Destroy(LeaderboardCards[i]);
        }
        LeaderboardCards = new List<GameObject>();
    }

    #region Developer 2
    public int[] MaxPlayer = { 2, 4 };
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] GameObject CreateRoomPanel;
    [SerializeField] GameObject PlayerListPanel;
    [SerializeField] TextMeshProUGUI RoomNameText;
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] Transform PlayerContent;
    public TextMeshProUGUI MaxPlayerText;
    [SerializeField] public GameObject RoomPanel;
    [SerializeField] public GameObject BottomPanel;

    [SerializeField] public GameObject PlayerGameInfoScrollView;
    [SerializeField] public GameObject PlayerGameInfoPrefab;
    [SerializeField] public Transform PlayerGameInfoContent;
    private void UpdatePlayerData()
    {
        if (PhotonNetwork.IsConnected)
            PlayerName.text = PhotonNetwork.LocalPlayer.NickName;
        //MaxPlayerText.text = GlobalData.MaxPlayer.ToString();
    }
    int currentMaxPlayerIndex = 0;
    public void onNextChangeMaxPlayer()
    {
        currentMaxPlayerIndex++;
        if(currentMaxPlayerIndex >= MaxPlayer.Length)
        {
            currentMaxPlayerIndex = 0;
        }
        //MaxPlayerText.text = MaxPlayer[currentMaxPlayerIndex].ToString();
        GlobalData.MaxPlayer = currentMaxPlayerIndex;
    }
    public void OnPrevChangeMaxPlayer()
    {
        currentMaxPlayerIndex--;
        if(currentMaxPlayerIndex < 0)
        {
            currentMaxPlayerIndex = MaxPlayer.Length - 1;
        }
        //MaxPlayerText.text = MaxPlayer[currentMaxPlayerIndex].ToString();
        GlobalData.MaxPlayer = currentMaxPlayerIndex;
    }
    public void OnClickOnCreateRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            string roomName = "Room# " + Random.Range(1, 9999);
            //string roomName = "Room#";
            //PhotonAuth.Instance.JoinOrCreateRoom(roomName, GlobalData.MaxPlayer);
            UIManager.instance.PlayerListPanel.SetActive(true);
            UIManager.instance.CreateRoomPanel.SetActive(false);
            PhotonAuth.Instance.JoinOrCreateRoom();
        }
    }
    List<GameObject> playerList = new List<GameObject>();
    public void RoomPlayerList()
    {
        ClearPlayerList();
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            GameObject Init = Instantiate(PlayerPrefab, PlayerContent);
            Init.GetComponent<PlayerInfo>().TextPlayerNumber.text = (i + 1).ToString();
            Init.GetComponent<PlayerInfo>().TextPlayerName.text = players[i].NickName.ToString();
            playerList.Add(Init);
            //Debug.Log($"Player Name: {players[i].NickName}, Master Client: {players[i].IsMasterClient}");
        }
        if (PhotonNetwork.IsMasterClient)
        {
            LetStart.SetActive(true);
        }
        else
        {
            LetStart.SetActive(false);
        }
    }
    void ClearPlayerList()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            Destroy(playerList[i]);
        }
        playerList = new List<GameObject>();
    }
    #endregion
    [SerializeField] GameObject Player;
    [SerializeField] GameObject LetStart;
    public void OnClickStart()
    {
        photonView.RPC(nameof(StartGame), RpcTarget.All);
    }
    [PunRPC]
    void StartGame()
    {
        RoomPanel.SetActive(false);
        BottomPanel.SetActive(true);
        PlayerGameInfoScrollView.SetActive(true);
        Player.SetActive(true);
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
}
