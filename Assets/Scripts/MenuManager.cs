using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

public GameObject SettingsPanel;
    void Start()
    {
        SettingsPanel.SetActive(false);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
