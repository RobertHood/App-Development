using UnityEngine;
using System.Collections.Generic;
public class Lives : MonoBehaviour
{
    public List<GameObject> error_images;
    int lives_ = 0;
    int error_number_ = 3;
    public GameObject gameOverPopup;
    void Start()
    {
        lives_ = (error_images != null) ? error_images.Count : 0;
        error_number_ = 0;
    }

    private void WrongNumber()
    {
        if (error_number_ < error_images.Count)
        {
            error_images[error_number_].SetActive(true);
            error_number_++;
            lives_--;
        }
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (lives_ <= 0)
        {
            GameEvents.OnGameOverMethod();
            gameOverPopup.SetActive(true);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnWrongNumber += WrongNumber;
    }

    private void OnDisable()
    {
        GameEvents.OnWrongNumber -= WrongNumber;
    }
}
