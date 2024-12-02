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
        }
    }
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = "Player_123", // Replace with a unique player identifier
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Successful!");
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
        DeleteLeaderBoard();
        foreach (var entry in result.Leaderboard)
        {
            Debug.Log($"Rank: {entry.Position}, Player: {entry.DisplayName ?? entry.PlayFabId}, Score: {entry.StatValue}");
            GameObject card = Instantiate(ScoreCardPrefab, Content);
            card.GetComponent<CardInfo>().rank.text = (entry.Position + 1).ToString();
            card.GetComponent<CardInfo>().playerName.text = entry.DisplayName ?? entry.PlayFabId;
            card.GetComponent<CardInfo>().playerScore.text = entry.StatValue.ToString();
            LeaderboardCards.Add(card);
        }
    }

    void OnLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError("Failed to fetch leaderboard: " + error.ErrorMessage);
    }
    [SerializeField] GameObject ScoreCardPrefab;
    [SerializeField] Transform Content;
    [SerializeField] List<GameObject> LeaderboardCards = new List<GameObject>();
    public void DeleteLeaderBoard()
    {
        for (int i = 0; i < LeaderboardCards.Count; i++)
        {
            Destroy(LeaderboardCards[i]);
        }
        LeaderboardCards = new List<GameObject>();
    }
}
