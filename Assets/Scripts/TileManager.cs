using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    private TileInfo currentTileInfo;
    [Header("Money Reward")]
    public TextMeshProUGUI moneyText;


    [Header("Shield Reward")]
    public TextMeshProUGUI shieldText;


    [Header("BankHiest Reward")]
    public TextMeshProUGUI bankHiest;


    [Header("Attack Reward")]
    public TextMeshProUGUI attack;

    [Header("CommunityChest Reward")]
    public GameObject communityResourcesPanel;
    public TextMeshProUGUI communityResourceTitle;
    public TextMeshProUGUI communityResourceDesciption;

    public Chances[] communityResourcesData;

    [Header("Chance Reward")]
    public GameObject ChancePanels;
    public TextMeshProUGUI chanceTitle;
    public TextMeshProUGUI chanceDescription;

    public Chances[] communityChancesData;


    [Header("Jail")]
    public TextMeshProUGUI diceNumber;
    public GameObject diceNumBox;
    public GameObject youAreFree;
    public int diceToRelease;
    public int fineToRelease;
    public GameObject JaiLPanel;
    public TextMeshProUGUI JailTitle;
    public TextMeshProUGUI JailDescription;

    public JailData[] jailData;
    [System.Serializable]
    public class Chances
    {
        public string Title;
        public string Description;
        public int money;
    }
    [System.Serializable]
    public class CommunityResource
    {
        public string Title;
        public string Description;
        public int money;
    }

    [System.Serializable]
    public class JailData
    {
        public string Title;
        public string Description;
        public int diceToRelease;
        public int fineToRelease;
    }
    public enum ChanceType
    {
        GrandFatherGaveMoney,
        EightSpin,
        NineSpin,
        TourToHeist,
        TourToShutDown
    }

    void Start()
    {
        instance = this;
    }

    public void GiveTileReward(TileInfo tileInfo)
    {
        currentTileInfo = tileInfo;
        GlobalData.TileName tileName = currentTileInfo.tileName;
        switch (tileName)
        {
            case GlobalData.TileName.Money:
                GiveMoneyReward();
                break;

            case GlobalData.TileName.Attack:
                GiveAttackReward();
                break;
            case GlobalData.TileName.BankHiest:
                GiveBankHeistReward();
                break;

            case GlobalData.TileName.Shield:
                GiveShieldReward();
                break;
            case GlobalData.TileName.CommunityChest:
                GiveCommunityChest();
                break;
            case GlobalData.TileName.DecreaseMoney:
                DecreaseMoney();
                break;
            case GlobalData.TileName.Chance:
                GiveChance();
                break;
            case GlobalData.TileName.Parking:
                Parking();
                break;
            case GlobalData.TileName.GoToJail:
                GoToJail();
                break;
            case GlobalData.TileName.Go:
                GiveMoneyReward();
                break;

            default:
                Debug.LogError("Unknown tile name: " + tileName);
                break;
        }
    }

    private void OnLandGo()
    {
        UIManager.instance.moneyPanelText.GetComponent<TextMeshProUGUI>().color = Color.green;

        UIManager.instance.moneyPanelText.text = currentTileInfo.money.ToString();
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        moneyText.text = currentTileInfo.money.ToString();
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        Debug.Log("Giving Go reward!");

        // Also one extra dice
    }
    private void GiveMoneyReward()
    {
        // Implement logic to give money reward
        UIManager.instance.moneyPanelText.GetComponent<TextMeshProUGUI>().color = Color.green;

        UIManager.instance.moneyPanelText.text = currentTileInfo.money.ToString();
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        moneyText.text = currentTileInfo.money.ToString();
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        UIManager.instance.moneyPanel.gameObject.SetActive(true);
        Debug.Log("Giving money reward!");

    }
    //
    private void GiveAttackReward()
    {
        UIManager.instance.attackPanel.gameObject.SetActive(true);

        // Implement logic for attack reward
        Debug.Log("Giving attack reward!");
    }

    private void GiveBankHeistReward()
    {
        UIManager.instance.bankHiestPanel.gameObject.SetActive(true);

        // Implement logic for bank heist reward
        Debug.Log("Giving bank heist reward!");
    }

    private void GiveShieldReward()
    {
        GlobalData.SetShields(GlobalData.GetShields() + currentTileInfo.shield);

        shieldText.text = currentTileInfo.shield.ToString();
        UIManager.instance.shieldPanel.gameObject.SetActive(true);

        // Implement logic for shield reward
        Debug.Log("Giving shield reward!");
    }
    private void GiveCommunityChest()
    {
        int rndChance = UnityEngine.Random.Range(0, communityResourcesData.Length);
        communityResourceDesciption.text = communityResourcesData[rndChance].Description;
        communityResourceTitle.text = communityResourcesData[rndChance].Title;
        communityResourcesPanel.SetActive(true);
        UIManager.instance.UpdateMoneyInMatch(communityResourcesData[rndChance].money);
        // Implement logic for shield reward
        Debug.Log("Giving Community Resource Reward!");
    }

    private void GiveChance()
    {
        int rndChance = UnityEngine.Random.Range(0, communityChancesData.Length);
        chanceDescription.text = communityChancesData[rndChance].Description;
        chanceTitle.text = communityChancesData[rndChance].Title;
        ChancePanels.SetActive(true);
        UIManager.instance.UpdateMoneyInMatch(communityChancesData[rndChance].money);
        // Implement logic for shield reward
        Debug.Log("Giving Chance reward!");
    }

    private void DecreaseMoney()
    {
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        UIManager.instance.moneyPanelText.text = currentTileInfo.money.ToString();
        UIManager.instance.moneyPanelText.GetComponent<TextMeshProUGUI>().color = Color.red;
        UIManager.instance.moneyPanel.SetActive(true);
        // Implement logic for shield reward
        Debug.Log("Decrease Money!");
    }

    private void Parking()
    {
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        UIManager.instance.moneyPanelText.text = currentTileInfo.money.ToString();
        UIManager.instance.moneyPanelText.GetComponent<TextMeshProUGUI>().color = Color.red;
        UIManager.instance.moneyPanel.SetActive(true);
    }

    private void GoToJail()
    {
        switch (MonopolyGo.instance.playerClass)
        {
            case MonopolyGo.PlayerClass.UpperClass:
                JailDescription.text = jailData[0].Description;
                JailTitle.text = jailData[0].Title;
                diceToRelease = jailData[0].diceToRelease;
                fineToRelease = jailData[0].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
            case MonopolyGo.PlayerClass.MiddleClass:
                JailDescription.text = jailData[1].Description;
                JailTitle.text = jailData[1].Title;
                diceToRelease = jailData[1].diceToRelease;
                fineToRelease = jailData[1].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
            case MonopolyGo.PlayerClass.WorkingClass:
                JailDescription.text = jailData[2].Description;
                JailTitle.text = jailData[2].Title;
                diceToRelease = jailData[2].diceToRelease;
                fineToRelease = jailData[2].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
            case MonopolyGo.PlayerClass.LowerClass:
                JailDescription.text = jailData[3].Description;
                JailTitle.text = jailData[3].Title;
                diceToRelease = jailData[3].diceToRelease;
                fineToRelease = jailData[3].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
        }
    }

    public void PayJailFine()
    {
        UIManager.instance.UpdateMoneyInMatch(-fineToRelease);
        JaiLPanel.SetActive(false);
        print("ReleaseFromJail");

    }
    public void RollDiceToRelease()
    {
        int rollDiceNumber = 0;
        diceNumBox.SetActive(true);
        if (MonopolyGo.instance.playerClass == MonopolyGo.PlayerClass.UpperClass || MonopolyGo.instance.playerClass == MonopolyGo.PlayerClass.LowerClass)
        {
            rollDiceNumber = Random.RandomRange(0, 7);
            if (rollDiceNumber % 2 == 0)
            {
                print("ReleaseFromJail");
                JaiLPanel.SetActive(false);
                diceToRelease = 0;
                Debug.Log(rollDiceNumber + " is even.");
            }
            else
            {
                Debug.Log(rollDiceNumber + " is odd.");
            }

            diceToRelease--;
            if (diceToRelease <= 0)
            {
                print("ReleaseFromJail");
                JaiLPanel.SetActive(false);
                youAreFree.SetActive(false);
            }
        }
        else
        {
            rollDiceNumber = Random.RandomRange(0, 7);
            if (rollDiceNumber % 2 == 0)
            {
                print("ReleaseFromJail");
                JaiLPanel.SetActive(false);
                youAreFree.SetActive(false);

                diceToRelease = 0;
                Debug.Log(rollDiceNumber + " is even.");
            }
            else
            {
                Debug.Log(rollDiceNumber + " is odd.");
            }
        }

    }




}
