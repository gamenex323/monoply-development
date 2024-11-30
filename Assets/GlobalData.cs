using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData 
{
    public enum TileName
    {
        Money,
        Attack,
        BankHiest,
        Shield
    }

    public static void ChangeMoney(float money)
    {
        UIManager.instance.globalMoney.text = money.ToString();
    }

    
}
