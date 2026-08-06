using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject youDiedPanel;
    [SerializeField] private GameObject jumpscareVideoPanel;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip jumpscareVideo;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] public GameObject PauseScreenPanel;
    [SerializeField] private GameObject AudioSettingsPanel;
    [SerializeField] private GameObject PlayerMovement;
    [SerializeField] private AudioMixer audioMixer; // Reference to the AudioMixer for volume control 

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

        if (AudioSettingsPanel != null)
        {
            AudioSettingsPanel.SetActive(false);
        }
        if (PauseScreenPanel != null)
        {
            PauseScreenPanel.SetActive(false);

        }

        // Setup video player to call ShowDeathScreen when video ends
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    [System.Obsolete]
    private void Update()
    {
        // Toggle pause screen with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = PauseScreenPanel != null && PauseScreenPanel.activeSelf;
            if (PauseScreenPanel != null)
            {
                if (youDiedPanel.activeSelf)
                {
                    return; // Don't allow pausing if the player has died
                }
                else if (jumpscareVideoPanel.activeSelf)
                {
                    return; // Don't allow pausing if the jumpscare video is playing
                }
                else
                {
                    PauseScreenPanel.SetActive(!isActive);
                    Time.timeScale = isActive ? 1f : 0f; // Resume or pause the game
                    Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.Confined;
                    Cursor.visible = !isActive;
                    audioMixer.SetFloat("TrueMasterVolume", isActive ? 0f : -80f); // Mute or unmute audio
                    PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                    if (playerMovement != null)
                    {
                        playerMovement.enabled = isActive; // Enable or disable player movement
                    }
                }
                if (youDiedPanel != null)
                {
                    youDiedPanel.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("GameManager: YouDiedPanel is not assigned in the inspector.");
                }
                if (PauseScreenPanel != null && PauseScreenPanel.activeSelf)
                {
                    AudioSettingsPanel.SetActive(false); // Close audio settings if pause screen is active
                }
            }
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
        PlayerMovement playerMovement = Object.FindAnyObjectByType<PlayerMovement>();
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
    public void OpenAudioSettings()
    {
        if (AudioSettingsPanel != null)
        {
            AudioSettingsPanel.SetActive(true);
        }
    }
    public void CloseAudioSettings()
    {
        if (AudioSettingsPanel != null)
        {
            AudioSettingsPanel.SetActive(false);
        }
    }
    public void ResumeGame()
    {
        if (PauseScreenPanel != null)
        {
            PauseScreenPanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            audioMixer.SetFloat("TrueMasterVolume", 0f); // Unmute audio
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true; // Enable player movement
            }
        }
    }

    public void SetMasterVolume()
    {
        float value = Mathf.Max(masterVolumeSlider.value, 0.0001f);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
    }

    public void SetMusicVolume()
    {
        float value = Mathf.Max(musicVolumeSlider.value, 0.0001f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
    }

    public void SetSFXVolume()
    {
        float value = Mathf.Max(sfxVolumeSlider.value, 0.0001f);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
    }

    public void ReturntoMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is running
        SceneManager.LoadScene("Menu");
        audioMixer.SetFloat("TrueMasterVolume", 0f);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#endif
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
