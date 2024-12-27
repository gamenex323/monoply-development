using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isJailPanel;
    public Button payFine;
    public Button rollDice;
    private void OnEnable()
    {
        Enable();
    }
    void Enable()
    {
        if (isJailPanel && AiMatchFinding.instance.turnOfPlayer != 0)
        {
            payFine.interactable = false;
            rollDice.interactable = false;
            JailPlay();
        }
        else
        {
            payFine.interactable = true;
            rollDice.interactable = true;
        }

    }

    // Update is called once per frame
    void JailPlay()
    {

        Debug.Log("Paying Jail Fine By AI");
        DG.Tweening.DOVirtual.DelayedCall(1.5f, () => TileManager.instance.PayJailFine());
    }
}
