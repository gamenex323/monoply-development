using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class GlobalData
{
    public enum TileName
    {
        Go,
        Money,
        Attack,
        BankHiest,
        Shield,
        CommunityChest,
        Visiting,
        DecreaseMoney,
        GoToJail,
        Chance,
        Parking,
        Shutdown,
        WaterSupply,
        Property
    }

    public enum PropertyColor
    {
        Red,
        Purple,
        LightBlue,
        Maroon,
        Orange

    }

    public static string MoneyKey = "PlayerMoney";
    public static string ShieldKey = "PlayerShields";
    public static string CommmunityChest = "CommmunityChest";

    // Set money value and save it to PlayerPrefs

    public static void SetMoney(int money)
    {
        PlayerPrefs.SetInt(MoneyKey, GetMoney() + money);
        PlayerPrefs.Save();
        if (PlayfabLogin.login)
            PlayfabLogin.login.SubmitScore(GetMoney());
    }
    // Get money value from PlayerPrefs
    public static int GetMoney()
    {
        return PlayerPrefs.GetInt(MoneyKey, 0);
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

    [Serializable]
    public class PlayerClassData
    {
        public MonopolyGo.PlayerClass playerClass;
        public int playerMoney;
        public bool playerEliminated;
        public string playerName;
    }
    // Coroutine for typewriter effect
    public static int Sound
    {
        set { PlayerPrefs.SetInt(_Sound, value); }
        get { return PlayerPrefs.GetInt(_Sound); }
    }
    public static int Music
    {
        set { PlayerPrefs.SetInt(_Music, value); }
        get { return PlayerPrefs.GetInt(_Music); }
    }
    #region Photon Data 
    private static string _NickName = "NickName";
    private static string _MaxPlayer = "MaxPlayer";
    private static string _Sound = "__Sound";
    private static string _Music = "_Music";
    public static string NickName
    {
        set { PlayerPrefs.SetString(_NickName, value); }
        get { return PlayerPrefs.GetString(_NickName); }
    }
    public static int MaxPlayer
    {
        set { PlayerPrefs.SetInt(_MaxPlayer, value); }
        get { return PlayerPrefs.GetInt(_MaxPlayer); }
    }
    public enum Modes
    {
        AI,
        Local,
        Multiplayer
    }
    #endregion



}
