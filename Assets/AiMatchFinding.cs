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
    public GlobalData. PlayerClassData[] PlayerClassData;
    public GameObject playButton;
    public PlayerClass[] playerTurnSequence ;
    public GameObject[] playersProfiles;
    public int turnOfPlayer = 0;
    public bool AiMatchIsPlaying;
    public GameObject upperPlayerProfiles;
    public GameObject panelData;
    public MonopolyGo player;
    public int eliminatedPlayers = 0;
    public static AiMatchFinding instance;
    private void OnEnable()
    {
        if(instance == null)
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
       
            players[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =PlayerClass.UpperClass.ToString();

            yield return new WaitForSeconds(1);
            players[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[0].playerMoney;
            players[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =PlayerClass.MiddleClass.ToString();
            playerTurnSequence[0] = PlayerClassData[0].playerClass;

            yield return new WaitForSeconds(1);
            players[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[1].playerMoney;
            players[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =PlayerClass.WorkingClass.ToString();
            playerTurnSequence[1] = PlayerClassData[1].playerClass;

            yield return new WaitForSeconds(1);
            players[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Your Initial Money: " + PlayerClassData[2].playerMoney;
            players[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =PlayerClass.LowerClass.ToString();
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
        UIManager.instance.bottomPanel.SetActive(true );
    }

    public void SwitchTurn()
    {

        player.gameObject.SetActive(true);
        player.playerIcon.SetActive(true);
        if (turnOfPlayer == 0)
        {
            MonopolyGo.instance.goButton.interactable = true;
        }
        else
        {

            MonopolyGo.instance.goButton.interactable = false;
            DG.Tweening.DOVirtual.DelayedCall(3, () => MonopolyGo.instance.OnGoButtonClicked());

        }
        MonopolyGo.instance.playerClass = playerTurnSequence[turnOfPlayer];
        foreach (GameObject player in playersProfiles)
        {
            player.GetComponent<Image>().color = Color.white;
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
            if (PlayerClassData[i].playerMoney<= 0 && !PlayerClassData[i].playerEliminated) {

                eliminatedPlayers++;
                PlayerClassData[i].playerEliminated = true;
                print("Player " + i+ " is Eliminated");
                playersProfiles[i].GetComponent<Image>().color = Color.red;
            }
            playersProfiles[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerClassData[i].playerMoney.ToString();
            playersProfiles[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerClassData[i].playerClass.ToString();
        }

        if (eliminatedPlayers >= 3)
        {
            for (int i = 0; i < PlayerClassData.Length; i++)
            {
                if (PlayerClassData[0].playerEliminated)
                {
                    AiMatchIsPlaying = false;
                    UIManager.instance.winnerName.text = PlayerClassData[0].playerName;
                    UIManager.instance.winnerPanel.SetActive(true);
                    DG.Tweening.DOVirtual.DelayedCall(3, () => SceneManager.LoadScene(1));
                }
                
            }
        }

    }
    public void TurnIncremented()
    {
        if (turnOfPlayer == noOfPlayers - 1)
        {
            turnOfPlayer = 0;
        }
        else
        {
            turnOfPlayer++;
            if (PlayerClassData[turnOfPlayer].playerMoney <= 0)
            {
                turnOfPlayer++;

            }
        }
        SwitchTurn();
    }
}
