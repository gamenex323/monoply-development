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

            default:
                Debug.LogError("Unknown tile name: " + tileName);
                break;
        }
    }

    private void GiveMoneyReward()
    {
        // Implement logic to give money reward
        GlobalData.SetMoney(GlobalData.GetMoney()+currentTileInfo.money);
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


}
