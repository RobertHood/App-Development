using UnityEngine;
using UnityEngine.SceneManagement;

public class AppLogicManager : MonoBehaviour
{
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
}
