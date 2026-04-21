using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public VideoPlayer introVideo;
    public GameObject menuPanel;
    
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;
    private bool isVideoPlaying = true;
    
    void Start()
    {
        menuPanel.SetActive(false);
        
        introVideo.loopPointReached += FinishVideo;
        introVideo.Play();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (isVideoPlaying && Input.GetKeyDown(KeyCode.Mouse0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickThreshold)
            {
                FinishVideo(introVideo); // Skip on double click
            }

            lastClickTime = Time.time;
        }
    }

    void FinishVideo(VideoPlayer vp)
    {
        isVideoPlaying = false;
        introVideo.gameObject.SetActive(false); // Hide the video player
        menuPanel.SetActive(true);              // Show the buttons
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
}