using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int score;
    public GameObject playButton;
    public GameObject gameOver;
    public GameObject getReady;
    public Text scoreText;
    public Player player;

    private void Awake(){
        Application.targetFrameRate = 60;
        Pause();
        getReady.SetActive(true);
        gameOver.SetActive(false);
    }

    public void Play(){
        score = 0;
        scoreText.text = score.ToString();
        
        getReady.SetActive(false);
        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;
        
        Pipes[] pipes = FindObjectsOfType<Pipes>();
        for(int i = 0; i< pipes.Length; i++){
            Destroy(pipes[i].gameObject);
        }
    }

    public void Pause(){
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void GameOver(){
        gameOver.SetActive(true);
        playButton.SetActive(true);
        Pause();
    }

    public void IncreaseScore(){
        score++;
        scoreText.text = score.ToString();
    }
}
