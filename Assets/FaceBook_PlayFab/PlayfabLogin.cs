using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabLogin : MonoBehaviour
{
    public static PlayfabLogin login;
    private void Awake()
    {
        if(login == null)
        {
            login = this;
            DontDestroyOnLoad(this);
        }
    }
    void Start()
    {
        //Login();
    }

    public void Login()
    {
        print("GueatLogin");
        if (GlobalData.NickName == "")
        {
            string name = "Guest " + Random.Range(1, 9999);
            GlobalData.NickName = name;
        }

        
        var request = new LoginWithCustomIDRequest
        {
            CustomId = GlobalData.NickName, // Replace with a unique player identifier
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Successful!");
        PhotonAuth.Instance.AuthenticateWithPlayFab();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login Failed: " + error.ErrorMessage);
    }
    
    // Set Leaderboard
    public void SubmitScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate { StatisticName = "PlayerScore", Value = score }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreUpdateSuccess, OnScoreUpdateFailure);
    }

    void OnScoreUpdateSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score updated successfully!");
    }

    void OnScoreUpdateFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update score: " + error.ErrorMessage);
    }
    
    // Get Leaderboard....
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlayerScore",
            StartPosition = 0,
            MaxResultsCount = 10 // Adjust to fetch more or fewer entries
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, OnLeaderboardFailure);
    }

    void OnLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("Leaderboard fetched successfully!");
        UIManager.instance.DeleteLeaderBoard();
        foreach (var entry in result.Leaderboard)
        {
            Debug.Log($"Rank: {entry.Position}, Player: {entry.DisplayName ?? entry.PlayFabId}, Score: {entry.StatValue}");
            GameObject card = Instantiate(UIManager.instance.ScoreCardPrefab, UIManager.instance.Content);
            card.GetComponent<CardInfo>().rank.text = (entry.Position + 1).ToString();
            card.GetComponent<CardInfo>().playerName.text = entry.DisplayName ?? entry.PlayFabId;
            card.GetComponent<CardInfo>().playerScore.text = entry.StatValue.ToString();
            UIManager.instance.LeaderboardCards.Add(card);
        }
    }

    void OnLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to fetch leaderboard: " + error.ErrorMessage);
    }
   
}
