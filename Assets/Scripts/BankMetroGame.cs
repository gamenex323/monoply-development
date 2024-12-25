using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BankMetroGame : MonoBehaviour
{
    public Safe[] safes;
    public Image[] moneyRewardIcon;
    public Image[] gemRewardIcon;
    public Image[] crownRewardIcon;
    public int economyMoneyReward = 2222;
    public int economyGemsReward = 2222;
    public int economyCrownReward = 2222;
    public Sprite crownSprite;
    public Sprite gemSprite;
    public Sprite moneySprite;

    public Sprite safeLockSprite;
    public Sprite safeUnLockSprite;
    public static BankMetroGame instance;

    public int moneyHiestCount = -1;
    public int gemHiestCount = -1;
    public int crownHiestCount = -1;
    public int tempRewardOnWin;
    public TextMeshProUGUI winAmount;
    public GameObject bankHiestComplete;
    private void Start()
    {

        instance = this;
        SetSafesRandomly();

    }

    public void SetSafesRandomly()
    {
        foreach (Safe safe in safes)
        {

            safe.SetReward(GetRandomRewardType());
        }

    }
    public static RevealRewardType GetRandomRewardType()
    {
        // Get all values of the enum
        Array values = Enum.GetValues(typeof(RevealRewardType));

        // Generate a random index
        int randomIndex = UnityEngine.Random.Range(0, values.Length);

        // Return the randomly selected enum value
        return (RevealRewardType)values.GetValue(randomIndex);
    }

    public enum RevealRewardType
    {
        Gems,
        Money,
        Crown
    }

    public void CheckHiest(RevealRewardType RewardType)
    {

        switch (RewardType)
        {
            case RevealRewardType.Gems:
                if (gemHiestCount == 2)
                {
                    Invoke(nameof(CompletePanel), 1.5f);
                    tempRewardOnWin = economyGemsReward;
                }
                break;
            case RevealRewardType.Money:
                if (moneyHiestCount == 2)
                {
                    Invoke(nameof(CompletePanel), 1.5f);

                    tempRewardOnWin = economyMoneyReward;

                }

                break;
            case RevealRewardType.Crown:
                if (crownHiestCount == 2)
                {
                    Invoke(nameof(CompletePanel), 1.5f);
                    tempRewardOnWin = economyGemsReward;

                }


                break;
        }
    }

    void CompletePanel()
    {
        winAmount.text = tempRewardOnWin.ToString();
        bankHiestComplete.SetActive(true);
    }
    public void CollectMoney()
    {
        UIManager.instance.UpdateMoneyInMatch(tempRewardOnWin);
        bankHiestComplete.SetActive(false);
    }

}
