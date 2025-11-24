using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AppLogic : MonoBehaviour
{
    public List<Button> difficultyButtons;
    public Color selectedColor = Color.red;
    public Color normalColor = new Color(0.0f,40.0f,243.0f,255.0f); 

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackToGameScreen()
    {
        SceneManager.LoadScene(SceneManager.GetSceneByName("All Game Menu").buildIndex);
    }

    public void SelectDifficulty(Button clickedButton)
    {
        if (difficultyButtons == null) return;

        for (int i = 0; i < difficultyButtons.Count; i++)
        {
            var btn = difficultyButtons[i];
            if (btn == null) continue;
            var img = btn.image;
            img.color = (btn == clickedButton) ? selectedColor : normalColor;
        }

        int idx = difficultyButtons.IndexOf(clickedButton);
        switch (idx)
        {
            case 0: SetGameMode(EGameMode.EASY); break;
            case 1: SetGameMode(EGameMode.MEDIUM); break;
            case 2: SetGameMode(EGameMode.HARD); break;
            case 3: SetGameMode(EGameMode.INSANE); break;
            default: SetGameMode(EGameMode.NOT_SET); break;
        }
    }

    public void LoadEasyGame(string name)
    {
        AppLogic.Instance.SetGameMode(AppLogic.EGameMode.EASY);
        SceneManager.LoadScene(name);
    }
    public void LoadMediumGame(string name)
    {
        AppLogic.Instance.SetGameMode(AppLogic.EGameMode.MEDIUM);
        SceneManager.LoadScene(name);
    }
    public void LoadHardGame(string name)
    {
        AppLogic.Instance.SetGameMode(AppLogic.EGameMode.HARD);
        SceneManager.LoadScene(name);
    }
    public void LoadInsaneGame(string name)
    {
        AppLogic.Instance.SetGameMode(AppLogic.EGameMode.INSANE);
        SceneManager.LoadScene(name);
    }

    public void ActivateObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void DeactivateObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    //-------------- Set game mode ---------------------
    public enum EGameMode
    {
        NOT_SET, EASY, MEDIUM, HARD, INSANE
    }

    public static AppLogic Instance;
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else Destroy(this);
    }

    private EGameMode _GameMode;
    void Start()
    {
        _GameMode = EGameMode.NOT_SET;

        // wire up buttons so each calls SelectDifficulty when clicked
        if (difficultyButtons != null)
        {
            for (int i = 0; i < difficultyButtons.Count; i++)
            {
                var btn = difficultyButtons[i];
                if (btn == null) continue;
                Button captured = btn; // capture for closure
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => SelectDifficulty(captured));
                // initialize colors (first selected if you like)
                btn.image.color = normalColor;
            }
        }
    }

    public void SetGameMode(EGameMode mode)
    {
        _GameMode = mode;
    }

    public void SetGameMode(string mode)
    {
        if (mode == "Easy") SetGameMode(EGameMode.EASY);
        else if (mode == "Medium") SetGameMode(EGameMode.MEDIUM);
        else if (mode == "Hard") SetGameMode(EGameMode.HARD);
        else if (mode == "Insane") SetGameMode(EGameMode.INSANE);
        else SetGameMode(EGameMode.NOT_SET);
    }

    public string GetGameMode()
    {
        switch (_GameMode)
        {
            case EGameMode.EASY: return "Easy";
            case EGameMode.MEDIUM: return "Medium";
            case EGameMode.HARD: return "Hard";
            case EGameMode.INSANE: return "Insane";
        }

        Debug.LogError("game level not included");
        return " ";
    }

}
