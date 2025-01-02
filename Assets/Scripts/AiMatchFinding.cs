using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MonopolyGo;

public class AiMatchFinding : MonoBehaviour
{
    // Start is called before the first frame update
    public int noOfPlayers;
    public GameObject[] players;
    public GlobalData.PlayerClassData[] PlayerClassData;
    public GameObject playButton;
    public PlayerClass[] playerTurnSequence;
    public GameObject[] playersProfiles;
    public TextMeshProUGUI[] playersClass;
    public TextMeshProUGUI[] playersMoney;
    public int turnOfPlayer = 0;
    public bool AiMatchIsPlaying;
    public GameObject upperPlayerProfiles;
    public GameObject panelData;
    public MonopolyGo player;
    public int eliminatedPlayers = 0;
    public bool youWon = false;
    public static AiMatchFinding instance;
    private void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    public void Initialize()
    {
        panelData.SetActive(true);
        playButton.SetActive(false);
        playerTurnSequence = new PlayerClass[noOfPlayers];
        StartCoroutine(FindingClassRandomly());
    }

    // Update is called once per frame
    IEnumerator FindingClassRandomly()
    {

        players[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerClass.HigherEarner.ToString();

        yield return new WaitForSeconds(1);
        players[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[0].playerMoney;
        players[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerClass.MiddleEarner.ToString();
        playerTurnSequence[0] = PlayerClassData[0].playerClass;

        yield return new WaitForSeconds(1);
        players[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[1].playerMoney;
        players[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerClass.EveryDayEarner.ToString();
        playerTurnSequence[1] = PlayerClassData[1].playerClass;

        yield return new WaitForSeconds(1);
        players[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[2].playerMoney;
        players[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerClass.EmergingEarner.ToString();
        playerTurnSequence[2] = PlayerClassData[2].playerClass;

        yield return new WaitForSeconds(1);
        players[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[3].playerMoney;
        playerTurnSequence[3] = PlayerClassData[3].playerClass;
        yield return new WaitForSeconds(0.5f);
        playButton.SetActive(true);


    }

    public void PlayAiMatch()
    {
        upperPlayerProfiles.SetActive(true);
        RefreshAllPlayersProfile();
        SwitchTurn();
        AiMatchIsPlaying = true;
        UIManager.instance.bottomPanel.SetActive(true);
    }

    public void SwitchTurn()
    {
        player.gameObject.SetActive(true);
        player.playerIcon.SetActive(true);
        if (turnOfPlayer == 0)
        {
            if (MonopolyGo.instance.currentPlayerIsInJail == true)
            {
                print("Current Player is in Jail: " + MonopolyGo.instance.currentPlayerIsInJail);
                DG.Tweening.DOVirtual.DelayedCall(1f, () => TileManager.instance.GoToJail());
                DG.Tweening.DOVirtual.DelayedCall(1.5f, () => TileManager.instance.JaiLPanel.SetActive(true));
            }
            else
            {
                MonopolyGo.instance.goButton.interactable = true;
            }
        }
        else
        {
            MonopolyGo.instance.goButton.interactable = false;
            DG.Tweening.DOVirtual.DelayedCall(3, () => MonopolyGo.instance.OnGoButtonClicked());
        }
        MonopolyGo.instance.playerClass = playerTurnSequence[turnOfPlayer];
        foreach (GameObject player in playersProfiles)
        {
            if (player.GetComponent<Image>().color != Color.red)
            {
                player.GetComponent<Image>().color = Color.white;
            }

        }
        playersProfiles[turnOfPlayer].GetComponent<Image>().color = Color.green;

    }

    public void UpdatePlayerProfile(int money)
    {

        PlayerClassData[turnOfPlayer].playerMoney += money;
        RefreshAllPlayersProfile();
    }

    void RefreshAllPlayersProfile()
    {
        for (int i = 0; i < playersProfiles.Length; i++)
        {
            if (PlayerClassData[i].playerMoney <= 0 && !PlayerClassData[i].playerEliminated)
            {

                eliminatedPlayers++;
                PlayerClassData[i].playerEliminated = true;
                print("Player " + i + " is Eliminated");
                playersProfiles[i].GetComponent<Image>().color = Color.red;
            }
            playersMoney[i].text = PlayerClassData[i].playerMoney.ToString();
            playersClass[i].text = PlayerClassData[i].playerClass.ToString();
        }

        if (eliminatedPlayers >= 3)
        {
            youWon = true;
            AiMatchIsPlaying = false;
            UIManager.instance.winnerName.text = PlayerClassData[0].playerName;
            UIManager.instance.winnerPanel.SetActive(true);
            DG.Tweening.DOVirtual.DelayedCall(3, () => SceneManager.LoadScene(1));

        }



        if (PlayerClassData[0].playerEliminated == true)
        {
            UIManager.instance.YouLoss.SetActive(true);
            DG.Tweening.DOVirtual.DelayedCall(3, () => SceneManager.LoadScene(1));
        }

    }
    public void TurnIncremented()
    {
        RefreshAllPlayersProfile();
        if (!youWon)
        {
            if (turnOfPlayer == noOfPlayers - 1)
            {
                turnOfPlayer = 0;
            }
            else
            {
                turnOfPlayer++;
                if (PlayerClassData[turnOfPlayer].playerEliminated == true)
                {
                    TurnIncremented();
                    return;

                }
            }
            SwitchTurn();
        }

    }
}
