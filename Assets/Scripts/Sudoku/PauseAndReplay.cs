using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseAndReplay : MonoBehaviour
{
    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackToGameScreen()
    {
        SceneManager.LoadScene("All Game Menu");
    }
    public void Continue(GameObject pauseUI)
    {
        pauseUI.SetActive(false);
        GameEvents.OnResumeGameMethod();
    }

    public void CallPauseUI(GameObject pauseUI)
    {
        pauseUI.SetActive(true);
        GameEvents.OnPauseGameMethod();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
