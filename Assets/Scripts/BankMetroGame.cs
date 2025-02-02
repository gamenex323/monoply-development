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
    public GameObject initialPanel;
    public AudioSource safeComingSound;
    private void OnEnable()
    {
        Enable();
    }
    private void Enable()
    {
        SettingManager.instance.directionLight.SetActive(false);
        if (!instance)
        {
            instance = this;
        }
        foreach (Image img in moneyRewardIcon)
        {
            img.color = Color.white;
        }
        foreach (Image img in gemRewardIcon)
        {
            img.color = Color.white;
        }
        foreach (Image img in crownRewardIcon)
        {
            img.color = Color.white;
        }
        moneyHiestCount = -1;
        gemHiestCount = -1;
        crownHiestCount = -1;
        SetSafesRandomly();

        DG.Tweening.DOVirtual.DelayedCall(2.5f, () => initialPanel.SetActive(false));
        DG.Tweening.DOVirtual.DelayedCall(0.5f, () => safeComingSound.Play());

    }

    public void SetSafesRandomly()
    {
        foreach (Safe safe in safes)
        {

            safe.SetReward(GetRandomRewardType());
            safe.Hide();
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
        DG.Tweening.DOVirtual.DelayedCall(0.5f, () => MonopolyGo.instance.EndTurn());

        UIManager.instance.UpdateMoneyInMatch(tempRewardOnWin);
        bankHiestComplete.SetActive(false);
    }

    private void OnDisable()
    {
        SettingManager.instance.directionLight.SetActive(true);

    }

}
