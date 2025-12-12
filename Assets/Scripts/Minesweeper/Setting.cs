using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Setting : MonoBehaviour
{
    [Header("Buttons")]
    public Button backButton;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public Button toHomeButton;

    [Header("Game Logic")]
    public GameLogic gameLogic;

    private void Start()
    {
        backButton.onClick.AddListener(OnBackClick);
        easyButton.onClick.AddListener(() => OnDifficultyClick(10));
        mediumButton.onClick.AddListener(() => OnDifficultyClick(20));
        hardButton.onClick.AddListener(() => OnDifficultyClick(30));
        toHomeButton.onClick.AddListener(ToHomeClick);
    }

    private void OnBackClick()
    {
        gameObject.SetActive(false);
    }

    private void OnDifficultyClick(int diff)
    {
        gameLogic.NewGameWithDiff(diff);

        gameObject.SetActive(false); 
    }

    private void ToHomeClick()
    {
        SceneManager.LoadScene("All Game Menu");
    }
    
}
