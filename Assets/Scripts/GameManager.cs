using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject youDiedPanel;
    [SerializeField] private GameObject jumpscareVideoPanel;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip jumpscareVideo;
    [SerializeField] private float deathDelay = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Ensure UI panels are hidden at the start
        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(false);
        }
        if (jumpscareVideoPanel != null)
        {
            jumpscareVideoPanel.SetActive(false);
        }

        // Setup video player to call ShowDeathScreen when video ends
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    public void PlayerDied()
    {
        // Hide the player
        if (playerModel != null)
        {
            playerModel.SetActive(false);
        }

        // Stop player movement
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Play jumpscare video
        PlayJumpscareVideo();
    }

    private void PlayJumpscareVideo()
    {
        if (jumpscareVideoPanel != null && videoPlayer != null && jumpscareVideo != null)
        {
            jumpscareVideoPanel.SetActive(true);
            videoPlayer.clip = jumpscareVideo;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("GameManager: Jumpscare video or panel not configured. Showing death screen immediately.");
            ShowDeathScreen();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        ShowDeathScreen();
    }

    private void ShowDeathScreen()
    {
        if (jumpscareVideoPanel != null)
        {
            jumpscareVideoPanel.SetActive(false);
        }

        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(true);
        }

        // Unlock and show the mouse cursor
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void RespawnGame()
    {
        Time.timeScale = 1f; // Ensure time is running
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
