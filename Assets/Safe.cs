using UnityEngine;
using UnityEngine.UI;

public class Safe : MonoBehaviour
{
    public Image rewardIcon;
    public Image safeImage;
    public BankMetroGame.RevealRewardType RewardType;

    public void SetReward(BankMetroGame.RevealRewardType newRewardType)
    {
        RewardType = newRewardType;
        switch (newRewardType)
        {
            case BankMetroGame.RevealRewardType.Gems:
                rewardIcon.sprite = BankMetroGame.instance.gemSprite;
                break;
            case BankMetroGame.RevealRewardType.Money:
                rewardIcon.sprite = BankMetroGame.instance.moneySprite;


                break;
            case BankMetroGame.RevealRewardType.Crown:
                rewardIcon.sprite = BankMetroGame.instance.crownSprite;

                break;
        }

    }

    public void Reveal()
    {
        rewardIcon.gameObject.SetActive(true);
        safeImage.sprite = BankMetroGame.instance.safeUnLockSprite;

        switch (RewardType)
        {
            case BankMetroGame.RevealRewardType.Gems:
                BankMetroGame.instance.gemHiestCount++;
                BankMetroGame.instance.gemRewardIcon[BankMetroGame.instance.gemHiestCount].color = Color.green;
                break;
            case BankMetroGame.RevealRewardType.Money:
                BankMetroGame.instance.moneyHiestCount++;
                BankMetroGame.instance.moneyRewardIcon[BankMetroGame.instance.moneyHiestCount].color = Color.green;


                break;
            case BankMetroGame.RevealRewardType.Crown:
                BankMetroGame.instance.crownHiestCount++;
                BankMetroGame.instance.crownRewardIcon[BankMetroGame.instance.crownHiestCount].color = Color.green;



                break;
        }

        BankMetroGame.instance.CheckHiest(RewardType);
    }
}
