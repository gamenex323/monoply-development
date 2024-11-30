using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class GlobalData
{
    public enum TileName
    {
        Money,
        Attack,
        BankHiest,
        Shield
    }

    public static string MoneyKey = "PlayerMoney";
    public static string ShieldKey = "PlayerShields";

    // Set money value and save it to PlayerPrefs

    public static void SetMoney(float money)
    {
        PlayerPrefs.SetFloat(GlobalData.MoneyKey, money);
        PlayerPrefs.Save();
    }
    // Get money value from PlayerPrefs
    public static float GetMoney()
    {
        return PlayerPrefs.GetFloat(MoneyKey, 0f);
    }

    // Set shield value and save it to PlayerPrefs
    public static void SetShields(int shields)
    {
        PlayerPrefs.SetInt(ShieldKey, shields);
        PlayerPrefs.Save();
        //if (UIManager.instance != null && UIManager.instance.globalShields != null)
        //{
        //    UIManager.instance.StartCoroutine(TypewriterEffect(UIManager.instance.globalShields, shields.ToString()));
        //}
    }

    // Get shield value from PlayerPrefs
    public static int GetShields()
    {
        return PlayerPrefs.GetInt(ShieldKey, 0);
    }

    // Coroutine for typewriter effect

}
