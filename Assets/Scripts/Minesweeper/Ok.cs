using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ok : MonoBehaviour
{
    [Header("Buttons")]
    public Button backButton;
    public Button toHomeButton;
    public Button replayButton;
    public TMP_Text messageTxt;
    [Header("Game Logic")]
    public GameLogic gameLogic;

    private void Start()
    {
        backButton.onClick.AddListener(OnBackClick);
        toHomeButton.onClick.AddListener(ToHomeClick);
        replayButton.onClick.AddListener(ReplayClick);
    }

    private void OnBackClick()
    {
        gameObject.SetActive(false);
    }

    private void ToHomeClick()
    {
        SceneManager.LoadScene("All Game Menu");
    }

    private void ReplayClick()
    {
        gameLogic.Replay();
    }

    public void UpdateMessageWin(int finalTime)
    {
        messageTxt.text = $"Your score is: {finalTime:000}";
    }

    public void UpdateMessageLose()
    {
        messageTxt.text = $"You Lose!";
    }
}
