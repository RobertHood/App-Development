using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Unity.Android.Gradle;
using JetBrains.Annotations;
using UnityEditor.ShaderGraph;

public class PlayFabLeaderBoard : MonoBehaviour
{
    public GameObject emptyLeaderboardText;

    public GameObject leaderboardContent;
    public GameObject BlockBlastLBPlayerCard;
    public GameObject SudokuLBPlayerCard;
    public GameObject MinesweeperLBPlayerCard;
    public GameObject FlappyBirdLBPlayerCard;

    void Awake()
    {
        GetBlockBlastLeaderboard();
    }
    public void sendLeaderboard(int score, string leaderboardName)
    {
        // guard: ensure manager exists and user is logged in
        if (PlayFabManager.Instance == null || !PlayFabManager.Instance.IsLoggedIn)
        {
            Debug.LogWarning("User is playing as guest → leaderboard not sent.");
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

    public void GetBlockBlastLeaderboard()
    {
        try
        {
            emptyLeaderboardText.SetActive(false);
            var request = new GetLeaderboardRequest
            {
            StatisticName = "block blast",
            StartPosition = 0,
            MaxResultsCount = 100
            };

            PlayFabClientAPI.GetLeaderboard(request, OnGetBlockBlastLeaderboardGet, OnError);
        }
        catch (Exception e)
        {
            emptyLeaderboardText.SetActive(true);
        }
        
    }

    void OnError(PlayFabError error)
    {
        Debug.LogWarning("Error retrieving leaderboard: " + error.GenerateErrorReport());
    }

    void OnGetBlockBlastLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (Transform child in leaderboardContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in result.Leaderboard)
        {
            GameObject card = Instantiate(BlockBlastLBPlayerCard, leaderboardContent.transform);
            card.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = item.DisplayName;
            card.transform.Find("UID").GetComponent<TextMeshProUGUI>().text = "UID: " + item.PlayFabId;
            card.transform.localScale = Vector3.one;
            if (item.Position + 1 == 1)
            {
                card.transform.Find("Placement").GetComponent<Image>().color = Color.gold;
            }
            else if (item.Position + 1 == 2)
            {
                card.transform.Find("Placement").GetComponent<Image>().color = Color.silver;
            }
            else if (item.Position + 1 == 3)
            {
                card.transform.Find("Placement").GetComponent<Image>().color = new Color(167f,112f,68f);
            }
            else card.transform.Find("Placement").GetComponent<Image>().color = new Color(0f,0f,0f);
            card.transform.Find("Placement").GetComponentInChildren<TextMeshProUGUI>().text = (item.Position + 1).ToString();
            card.transform.Find("HighScore").Find("HighScoreText").GetComponent<TextMeshProUGUI>().text = item.StatValue.ToString();

        }
    }
}