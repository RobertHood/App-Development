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

}
