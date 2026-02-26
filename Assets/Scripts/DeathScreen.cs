using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        scoreText.text = ScoreSystem.FinalScore.ToString("N0");
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Retry()
    {
        Debug.Log("Click");
        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("MainMenuScene");
    }
}