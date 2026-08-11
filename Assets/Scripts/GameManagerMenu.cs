using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManagerMenu : MonoBehaviour
{
    public static GameManagerMenu Instance { get; private set; }

    [SerializeField] public GameObject QualitySettingsPanel;
    [SerializeField] public GameObject ControlsSettingsPanel;
    [SerializeField] public GameObject MainSettingsPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private GameObject AudioSettingsPanel;
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
        if (AudioSettingsPanel != null)
        {
            AudioSettingsPanel.SetActive(false);
        }
        if (MainSettingsPanel != null)
        {
            MainSettingsPanel.SetActive(false);
        }
        if (QualitySettingsPanel != null)
        {
            QualitySettingsPanel.SetActive(false);
        }
        if (ControlsSettingsPanel != null)
        {
            ControlsSettingsPanel.SetActive(false);
        }

    }



    public void OpenMainSettings()
    {
        if (MainSettingsPanel != null)
        {
            MainSettingsPanel.SetActive(true);
        }
    }

    public void CloseMainSettings()
    {
        if (MainSettingsPanel != null)
        {
            MainSettingsPanel.SetActive(false);
        }
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
    public void OpenQualitySettings()
    {
        if (QualitySettingsPanel != null)
        {
            QualitySettingsPanel.SetActive(true);
        }
    }
    public void CloseQualitySettings()
    {
        if (QualitySettingsPanel != null)
        {
            QualitySettingsPanel.SetActive(false);
        }
    }
    public void OpenControlsSettings()
    {
        if (ControlsSettingsPanel != null)
        {
            ControlsSettingsPanel.SetActive(true);
        }
    }


    public void CloseControlsSettings()
    {
        if (ControlsSettingsPanel != null)
        {
            ControlsSettingsPanel.SetActive(false);
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
}
