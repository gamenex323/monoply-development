using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isJailPanel;
    void Start()
    {
        if (isJailPanel && AiMatchFinding.instance.turnOfPlayer!=0)
        {
            JailPlay();
        }
        
    }

    // Update is called once per frame
    void JailPlay()
    {
        DG.Tweening.DOVirtual.DelayedCall(5, () => TileManager.instance.PayJailFine());
    }
}
