using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppLogicManager : MonoBehaviour
{
    public GameObject errorMessage;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Settings()
    {

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false; // for editor mode
    }

    public void ToBlockBlastInstruction()
    {
        SceneManager.LoadScene("Block Blast - Instruction Screen");
    }

    public void ToMinesweeperInstruction()
    {
        SceneManager.LoadScene("Minesweeper - Instruction Screen");
    }

    public void ToSudokuInstruction()
    {
        SceneManager.LoadScene("Sudoku - Instruction Screen");
    }

    public void ToFlappyBirdInstruction()
    {
        SceneManager.LoadScene("Flappy Bird - Instruction Screen");
    }

    public void ToFlappyBird()
    {
        SceneManager.LoadScene("Flappy Bird");
    }

    public void ToBlockBlast()
    {
        SceneManager.LoadScene("Block Blast");
    }

    public void ToSudoku()
    {
        SceneManager.LoadScene("Sudoku");
    }

    public void ToMinesweeper()
    {
        SceneManager.LoadScene("Minesweeper");
    }
    
    public void ToAllGame()
    {
        SceneManager.LoadScene("All Game Menu");
    }
    public void EnableLeaderboardUI(GameObject leaderboardPanel)
    {
        leaderboardPanel.SetActive(true);
    }
    public void DisableLeaderboardUI(GameObject leaderboardPanel)
    {
        leaderboardPanel.SetActive(false);
    }
    public void EnableLoginPanel(GameObject loginPanel)
    {
        if (PlayFabManager.Instance != null && PlayFabManager.Instance.IsLoggedIn)
        {
            ShowErrorMessage("You are already logged in!");
        }
        else loginPanel.SetActive(true);
    }
    public void DisableLoginPanel(GameObject loginPanel)
    {
        loginPanel.SetActive(false);
    }
    public void EnableRegisterPanel(GameObject registerPanel)
    {
        if (PlayFabManager.Instance != null && PlayFabManager.Instance.IsLoggedIn)
        {
            ShowErrorMessage("You are already logged in!");
        }
        else registerPanel.SetActive(true);
    }
    public void DisableRegisterPanel(GameObject registerPanel)
    {
        registerPanel.SetActive(false);
    }

    private IEnumerator FadeOutNotification()
    {
        CanvasGroup cg = errorMessage.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = errorMessage.AddComponent<CanvasGroup>();
        }
        cg.alpha = 1f;
        yield return new WaitForSeconds(1f); 
        float duration = 0.5f;
        float time = 0f;
        while (time < duration && errorMessage != null && errorMessage.activeSelf)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }
        if (errorMessage != null)
        {
            cg.alpha = 0f;
            errorMessage.SetActive(false);
        }
    }

    private void ShowErrorMessage(String message){
        errorMessage.GetComponentInChildren<TextMeshProUGUI>().text = message;
        errorMessage.SetActive(true);
        StartCoroutine(FadeOutNotification());
    }
}
