using Photon.Pun;
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


    [Header("Property")]
    public GameObject propertyPanel;
    public TextMeshProUGUI propertyName;
    public TextMeshProUGUI propertyRent;
    public TextMeshProUGUI propertyPrice;
    public TextMeshProUGUI propertyStaus;
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
                MonopolyGo.instance.EndTurn();
                break;

            case GlobalData.TileName.Attack:
                GiveAttackReward();
                MonopolyGo.instance.EndTurn();

                break;
            case GlobalData.TileName.BankHiest:
                GiveBankHeistReward();
                break;

            case GlobalData.TileName.Shield:
                GiveShieldReward();
                MonopolyGo.instance.EndTurn();

                break;
            case GlobalData.TileName.CommunityChest:
                GiveCommunityChest();
                break;
            case GlobalData.TileName.DecreaseMoney:
                DecreaseMoney();
                MonopolyGo.instance.EndTurn();

                break;
            case GlobalData.TileName.Chance:
                GiveChance();
                break;
            case GlobalData.TileName.Parking:
                Parking();
                MonopolyGo.instance.EndTurn();

                break;
            case GlobalData.TileName.GoToJail:
                GoToJail();
                break;
            case GlobalData.TileName.Go:
                GiveMoneyReward();
                MonopolyGo.instance.EndTurn();

                break;
            case GlobalData.TileName.Property:
                GiveProperty();
               

                break;

            default:
                MonopolyGo.instance.EndTurn();
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
        UIManager.instance.moneyPanel.gameObject.SetActive(true);

        Debug.Log("Giving money reward!");

    }


    public void BuyProperty()
    {
        int tempBuyPrice = 0;
        if (currentTileInfo.money > 0)
        {
            tempBuyPrice = currentTileInfo.money;
        }
        else
        {
            tempBuyPrice = currentTileInfo.money * (-1);
        }

        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        currentTileInfo.propertyBuyByYou = true;
        MonopolyGo.instance.EndTurn();
        propertyStaus.text = "You Successfully Buy Property In " + tempBuyPrice;
        DG.Tweening.DOVirtual.DelayedCall(2f,()=> propertyPanel.SetActive(false));

    }
    public void RentProperty()
    {
        
        UIManager.instance.UpdateMoneyInMatch(-currentTileInfo.propertyRent);
        MonopolyGo.instance.EndTurn();
        propertyStaus.text = "You Rent Property In " + currentTileInfo.propertyRent;

        DG.Tweening.DOVirtual.DelayedCall(2f, () => propertyPanel.SetActive(false));

    }
    private void GiveProperty()
    {
        // Implement logic to give money reward

        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            if (AiMatchFinding.instance.turnOfPlayer == 0)
            {
                if (currentTileInfo.propertyBuyByYou)
                {
                    MonopolyGo.instance.EndTurn();
                }
                else
                {
                    propertyStaus.text = "";
                    propertyName.text = currentTileInfo.propertyName;
                    propertyPrice.text = "BUY: " + currentTileInfo.money;
                    propertyRent.text = "RENT: " + currentTileInfo.propertyRent;
                    propertyPanel.gameObject.SetActive(true);
                }

            }
            else
            {
                print("Turn Incremented From Here");
                ProceedPropertyTileByGivenValues();
            }

        }
        else
        {
            print("Turn Incremented From Here");

            ProceedPropertyTileByGivenValues();
        }






    }

    void ProceedPropertyTileByGivenValues()
    {
        if (currentTileInfo.money > 0)
        {
            UIManager.instance.propertyPanelText.GetComponent<TextMeshProUGUI>().color = Color.green;
            UIManager.instance.propertyPanelText.text = "You Got As Rent " + currentTileInfo.money.ToString() + " From Landed Property";
        }
        else
        {
            UIManager.instance.propertyPanelText.GetComponent<TextMeshProUGUI>().color = Color.red;
            UIManager.instance.propertyPanelText.text = "You Paid " + currentTileInfo.money.ToString() + " From Landed Property";

        }
        UIManager.instance.UpdateMoneyInMatch(currentTileInfo.money);
        moneyText.text = currentTileInfo.money.ToString();
        UIManager.instance.propertyPanel.gameObject.SetActive(true);
        Debug.Log("Giving Property reward!");

        MonopolyGo.instance.EndTurn();
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
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            if (AiMatchFinding.instance.turnOfPlayer == 0)
            {
                UIManager.instance.bankHiestPanel.gameObject.SetActive(true);
            }
            else
            {
                GiveMoneyReward();
                MonopolyGo.instance.EndTurn();
            }
        }
        else
        {
            UIManager.instance.bankHiestPanel.gameObject.SetActive(true);
        }




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
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            DG.Tweening.DOVirtual.DelayedCall(8, () => DisableComunityPanel());
        }
        else
        {
            communityResourcesPanel.SetActive(false);
            DG.Tweening.DOVirtual.DelayedCall(8, () => MonopolyGo.instance.EndTurn());
        }

    }

    void DisableComunityPanel()
    {
        if (communityResourcesPanel.activeInHierarchy)
        {
            MonopolyGo.instance.EndTurn();
            communityResourcesPanel.SetActive(false);
        }
    }

    public void GotItButton()
    {
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            communityResourcesPanel.SetActive(false);
            ChancePanels.SetActive(false);
            MonopolyGo.instance.EndTurn();
        }

    }
    void DisableChancesCardsPanel()
    {
        if (ChancePanels.activeInHierarchy)
        {
            MonopolyGo.instance.EndTurn();
            ChancePanels.SetActive(false);
            MonopolyGo.instance.EndTurn();
        }
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
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            DG.Tweening.DOVirtual.DelayedCall(8, () => DisableChancesCardsPanel());
        }
        else
        {
            ChancePanels.SetActive(false);
            DG.Tweening.DOVirtual.DelayedCall(8, () => MonopolyGo.instance.EndTurn());
        }
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

    public void GoToJail()
    {
        switch (MonopolyGo.instance.playerClass)
        {
            case MonopolyGo.PlayerClass.HigherEarner:
                JailDescription.text = jailData[0].Description;
                JailTitle.text = jailData[0].Title;
                diceToRelease = jailData[0].diceToRelease;
                fineToRelease = jailData[0].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
            case MonopolyGo.PlayerClass.MiddleEarner:
                if (AiMatchFinding.instance.AiMatchIsPlaying)
                {
                    if (AiMatchFinding.instance.turnOfPlayer == 0)
                    {
                        MonopolyGo.instance.currentPlayerIsInJail = true;
                    }
                }
                else
                {
                    MonopolyGo.instance.currentPlayerIsInJail = true;
                }
                JailDescription.text = jailData[1].Description;
                JailTitle.text = jailData[1].Title;
                diceToRelease = jailData[1].diceToRelease;
                fineToRelease = jailData[1].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
            case MonopolyGo.PlayerClass.EveryDayEarner:
                if (AiMatchFinding.instance.AiMatchIsPlaying)
                {
                    if (AiMatchFinding.instance.turnOfPlayer == 0)
                    {
                        MonopolyGo.instance.currentPlayerIsInJail = true;
                    }
                }
                else
                {
                    MonopolyGo.instance.currentPlayerIsInJail = true;
                }
                JailDescription.text = jailData[2].Description;
                JailTitle.text = jailData[2].Title;
                diceToRelease = jailData[2].diceToRelease;
                fineToRelease = jailData[2].fineToRelease;
                JaiLPanel.SetActive(true);
                break;
            case MonopolyGo.PlayerClass.EmergingEarner:
                if (AiMatchFinding.instance.AiMatchIsPlaying)
                {
                    if (AiMatchFinding.instance.turnOfPlayer == 0)
                    {
                        MonopolyGo.instance.currentPlayerIsInJail = true;
                    }
                }
                else
                {
                    MonopolyGo.instance.currentPlayerIsInJail = true;
                }
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
        if (AiMatchFinding.instance.AiMatchIsPlaying)
        {
            if (AiMatchFinding.instance.turnOfPlayer == 0)
            {
                MonopolyGo.instance.currentPlayerIsInJail = false;
            }
        }
        else
        {
            MonopolyGo.instance.currentPlayerIsInJail = false;
        }

        UIManager.instance.UpdateMoneyInMatch(-fineToRelease);
        JaiLPanel.SetActive(false);
        print("ReleaseFromJail");
        MonopolyGo.instance.EndTurn();

    }
    public void RollDiceToRelease()
    {

        int rollDiceNumber = 0;

        if (MonopolyGo.instance.playerClass == MonopolyGo.PlayerClass.HigherEarner || MonopolyGo.instance.playerClass == MonopolyGo.PlayerClass.EmergingEarner)
        {
            rollDiceNumber = Random.RandomRange(0, 7);
            diceNumBox.SetActive(true);
            diceNumber.text = rollDiceNumber.ToString();
            if (!MonopolyGo.instance.currentPlayerIsInJail)
            {

                if (rollDiceNumber % 2 == 0)
                {

                    if (AiMatchFinding.instance.AiMatchIsPlaying)
                    {
                        if (AiMatchFinding.instance.turnOfPlayer == 0)
                        {
                            MonopolyGo.instance.currentPlayerIsInJail = false;
                        }
                    }
                    else
                    {
                        MonopolyGo.instance.currentPlayerIsInJail = false;
                    }
                    print("ReleaseFromJail");
                    DG.Tweening.DOVirtual.DelayedCall(1.5f, () => JaiLPanel.SetActive(false));
                    diceToRelease = 0;
                    Debug.Log(rollDiceNumber + " is even.");
                    MonopolyGo.instance.EndTurn();
                    return;
                }
                else
                {
                    MonopolyGo.instance.diceRollToRelease = 2;
                    DG.Tweening.DOVirtual.DelayedCall(1.5f, () => JaiLPanel.SetActive(false));
                    if (AiMatchFinding.instance.AiMatchIsPlaying)
                    {
                        if (AiMatchFinding.instance.turnOfPlayer == 0)
                        {
                            MonopolyGo.instance.currentPlayerIsInJail = true;
                        }
                    }
                    else
                    {
                        MonopolyGo.instance.currentPlayerIsInJail = true;
                    }
                    Debug.Log(rollDiceNumber + " is odd.");

                }

            }
            else
            {
                MonopolyGo.instance.diceRollToRelease--;
                if (diceToRelease <= 0)
                {
                    if (AiMatchFinding.instance.AiMatchIsPlaying)
                    {
                        if (AiMatchFinding.instance.turnOfPlayer == 0)
                        {
                            MonopolyGo.instance.currentPlayerIsInJail = false;
                        }
                    }
                    else
                        MonopolyGo.instance.currentPlayerIsInJail = false;

                    print("ReleaseFromJail");


                    //youAreFree.SetActive(true);
                }
                DG.Tweening.DOVirtual.DelayedCall(1.5f, () => JaiLPanel.SetActive(false));
            }

        }
        else
        {
            rollDiceNumber = Random.RandomRange(0, 7);
            if (rollDiceNumber % 2 == 0)
            {
                if (AiMatchFinding.instance.AiMatchIsPlaying)
                {
                    if (AiMatchFinding.instance.turnOfPlayer == 0)
                    {
                        MonopolyGo.instance.currentPlayerIsInJail = false;
                    }
                }
                else
                    MonopolyGo.instance.currentPlayerIsInJail = false;

                print("ReleaseFromJail");
                JaiLPanel.SetActive(false);
                youAreFree.SetActive(false);

                diceToRelease = 0;
                Debug.Log(rollDiceNumber + " is even.");
            }
            else
            {
                Debug.Log(rollDiceNumber + " is odd.");
                DG.Tweening.DOVirtual.DelayedCall(1.5f, () => JaiLPanel.SetActive(false));
            }
        }
        MonopolyGo.instance.EndTurn();
    }







}
