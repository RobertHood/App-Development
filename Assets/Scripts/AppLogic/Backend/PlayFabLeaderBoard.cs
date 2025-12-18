using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

public enum LeaderboardType
{
    BlockBlast,
    Sudoku,
    Minesweeper,
    FlappyBird
}

public class PlayFabLeaderBoard : MonoBehaviour
{
    [Header("UI")]
    public GameObject emptyLeaderboardText;
    public RectTransform leaderboardContent;

    [Header("Player Card Prefabs")]
    public GameObject BlockBlastLBPlayerCard;
    public GameObject SudokuLBPlayerCard;
    public GameObject MinesweeperLBPlayerCard;
    public GameObject FlappyBirdLBPlayerCard;
    public TextMeshProUGUI GameName;


    private LeaderboardType currentLeaderboard = LeaderboardType.BlockBlast;

    void Awake()
    {
        currentLeaderboard = LeaderboardType.BlockBlast;
    }

    public void InitLeaderboardAfterLogin()
    {
        ShowCurrentLeaderboard();
    }

    public void NextLeaderboard()
    {
        currentLeaderboard =
            (LeaderboardType)(((int)currentLeaderboard + 1) % Enum.GetValues(typeof(LeaderboardType)).Length);

        ShowCurrentLeaderboard();
    }

    public void PreviousLeaderboard()
    {
        int total = Enum.GetValues(typeof(LeaderboardType)).Length;

        currentLeaderboard =
            (LeaderboardType)(((int)currentLeaderboard - 1 + total) % total);

        ShowCurrentLeaderboard();
    }

    public void UpdateGameName()
    {
        switch (currentLeaderboard)
        {
            case LeaderboardType.BlockBlast:
                GameName.text = "block blast";
                break;
            case LeaderboardType.Sudoku:
                GameName.text = "sudoku";
                break;
            case LeaderboardType.Minesweeper:
                GameName.text = "minesweeper";
                break;
            case LeaderboardType.FlappyBird:
                GameName.text = "flappy bird";
                break;
        }
    }

    public void ShowCurrentLeaderboard()
    {
        if (PlayFabManager.Instance == null ||
            !PlayFabManager.Instance.IsLoggedIn)
        {
            Debug.Log("Not logged in yet → skip leaderboard load");
            return;
        }
        UpdateGameName();
        switch (currentLeaderboard)
        {
            case LeaderboardType.BlockBlast:
                GetLeaderboard("block blast", BlockBlastLBPlayerCard);
                break;
            case LeaderboardType.Sudoku:
                GetLeaderboard("sudoku", SudokuLBPlayerCard);
                break;
            case LeaderboardType.Minesweeper:
                GetLeaderboard("minesweeper", MinesweeperLBPlayerCard);
                break;
            case LeaderboardType.FlappyBird:
                GetLeaderboard("flappy bird", FlappyBirdLBPlayerCard);
                break;
        }
    }

    

    void GetLeaderboard(string statisticName, GameObject cardPrefab)
    {
        emptyLeaderboardText.SetActive(false);

        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            result => DisplayLeaderboard(result, cardPrefab),
            OnError
        );
    }


    void DisplayLeaderboard(GetLeaderboardResult result, GameObject playerCardPrefab)
    {
        foreach (RectTransform child in leaderboardContent)
            Destroy(child.gameObject);

        if (result.Leaderboard == null || result.Leaderboard.Count == 0)
        {
            emptyLeaderboardText.SetActive(true);
            return;
        }

        foreach (var item in result.Leaderboard)
        {
            GameObject card = Instantiate(playerCardPrefab, leaderboardContent);
            card.transform.localScale = Vector3.one;

            card.transform.Find("PlayerName")
                .GetComponent<TextMeshProUGUI>().text =
                string.IsNullOrEmpty(item.DisplayName) ? "Anonymous" : item.DisplayName;

            card.transform.Find("UID")
                .GetComponent<TextMeshProUGUI>().text = "UID: " + item.PlayFabId;

            int rank = item.Position + 1;

            Image placementImage = card.transform.Find("Placement").GetComponent<Image>();
            if (rank == 1) placementImage.color = Color.gold;
            else if (rank == 2) placementImage.color = Color.silver;
            else if (rank == 3) placementImage.color = new Color(167f / 255f, 112f / 255f, 68f / 255f);
            else placementImage.color = Color.black;

            card.transform.Find("Placement")
                .GetComponentInChildren<TextMeshProUGUI>().text = rank.ToString();

            card.transform.Find("HighScore/HighScoreText")
                .GetComponent<TextMeshProUGUI>().text = item.StatValue.ToString();
        }
    }

    public void sendLeaderboard(int score, string leaderboardName)
    {
        if (PlayFabManager.Instance == null ||
            !PlayFabManager.Instance.IsLoggedIn)
        {
            Debug.LogWarning("Guest → leaderboard not sent");
            return;
        }

        if (string.IsNullOrWhiteSpace(leaderboardName))
            leaderboardName = "Score";

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

        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            _ => Debug.Log("Leaderboard updated"),
            error => Debug.LogWarning(error.GenerateErrorReport())
        );
    }

    void OnError(PlayFabError error)
    {
        emptyLeaderboardText.SetActive(true);
        Debug.LogWarning("Get leaderboard failed: " + error.GenerateErrorReport());
    }
}
