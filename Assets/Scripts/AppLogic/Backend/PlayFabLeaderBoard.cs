using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayFabLeaderBoard : MonoBehaviour
{
    public void sendLeaderboard(int score, string leaderboardName)
    {
        // guard: ensure manager exists and user is logged in
        if (PlayFabManager.Instance == null)
        {
            Debug.LogWarning("PlayFabManager.Instance is null. Cannot send leaderboard.");
            return;
        }

        if (!PlayFabManager.Instance.IsLoggedIn)
        {
            Debug.LogWarning("Cannot send leaderboard score: not logged in.");
            return;
        }

        // guard: ensure a valid statistic name
        if (string.IsNullOrWhiteSpace(leaderboardName))
        {
            Debug.LogWarning("Leaderboard name is empty; defaulting to 'Score'.");
            leaderboardName = "Score";
        }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdateSuccess, OnLeaderboardUpdateFailure);
    }

    void OnLeaderboardUpdateSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leaderboard updated successfully.");
    }

    void OnLeaderboardUpdateFailure(PlayFabError error)
    {
        Debug.LogWarning("Failed to update leaderboard: " + error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardGet, OnError);
    }

    void OnError(PlayFabError error)
    {
        Debug.LogWarning("Error retrieving leaderboard: " + error.GenerateErrorReport());
    }

    void OnGetLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(string.Format("Position: {0} | PlayFabId: {1} | Score: {2}", item.Position, item.PlayFabId, item.StatValue));
        }
    }
}