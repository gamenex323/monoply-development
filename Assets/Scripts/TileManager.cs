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

            default:
                Debug.LogError("Unknown tile name: " + tileName);
                break;
        }
    }

    private void GiveMoneyReward()
    {
        // Implement logic to give money reward
        UIManager.instance.moneyPanelText.GetComponent<TextMeshProUGUI>().color = Color.green;

        UIManager.instance.moneyPanelText.text = currentTileInfo.money.ToString();
        UIManager.instance.UpdateMoney(currentTileInfo.money);
        moneyText.text = currentTileInfo.money.ToString();
        UIManager.instance.UpdateMoney(currentTileInfo.money);
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
        UIManager.instance.UpdateMoney(communityResourcesData[rndChance].money);
        // Implement logic for shield reward
        Debug.Log("Giving Community Resource Reward!");
    }

    private void GiveChance()
    {
        int rndChance = UnityEngine.Random.Range(0, communityChancesData.Length);
        chanceDescription.text = communityChancesData[rndChance].Description;
        chanceTitle.text = communityChancesData[rndChance].Title;
        UIManager.instance.UpdateMoney(communityChancesData[rndChance].money);
        // Implement logic for shield reward
        Debug.Log("Giving Chance reward!");
    }

    private void DecreaseMoney()
    {
        UIManager.instance.UpdateMoney(currentTileInfo.money);
        UIManager.instance.moneyPanelText.text = currentTileInfo.money.ToString();
        UIManager.instance.moneyPanelText.GetComponent<TextMeshProUGUI>().color = Color.red;
        UIManager.instance.moneyPanel.SetActive(true);
        // Implement logic for shield reward
        Debug.Log("Decrease Money!");
    }

    private void Parking()
    {
        Debug.Log("Parking!");
    }




}
