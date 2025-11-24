using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class Setting : MonoBehaviour
{
    [Header("Buttons")]
    public Button backButton;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("Game Logic")]
    public GameLogic gameLogic;

    private void Start()
    {
        // Gán sự kiện cho nút
        backButton.onClick.AddListener(OnBackClick);
        easyButton.onClick.AddListener(() => OnDifficultyClick(10));
        mediumButton.onClick.AddListener(() => OnDifficultyClick(20));
        hardButton.onClick.AddListener(() => OnDifficultyClick(30));
    }

    private void OnBackClick()
    {
        gameObject.SetActive(false); // Ẩn panel
    }

    private void OnDifficultyClick(int diff)
    {
        gameLogic.NewGameWithDiff(diff);

        gameObject.SetActive(false); // Ẩn panel
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
