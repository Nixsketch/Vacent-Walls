using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CollectSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectCountText;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float soundVolume = 1f;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject FinalDoor; // Reference to the final door GameObject
    [SerializeField] private PlayableDirector finalDoorcutscene;

    private int collectCount = 0;
    private AudioSource audioSource;

    void Start()
    {
        if (FinalDoor == null && collectCount < 1)
        {
            Debug.LogWarning("CollectSystem: FinalDoor reference is not assigned!");
        }
        else
        {
            FinalDoor.SetActive(false); // Ensure the final door is initially closed
        }
        // Initialize the UI display
        if (collectCountText == null)
        {
            Debug.LogWarning("CollectSystem: collectCountText is not assigned!");
        }
        else
        {
            UpdateCountDisplay();
        }

        // Setup audio source if sound is assigned
        if (collectSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SoundFX")[0];
            }
        }
    }
    public void FinalDoorCutscene()
    {
        if (finalDoorcutscene != null)
        {
            finalDoorcutscene.Play();
            Debug.Log("Final door cutscene played!");
            FinalDoor.SetActive(true); // Open the final door after the cutscene
        }
        else
        {
            Debug.LogWarning("FinalDoorCutscene: finalDoorcutscene reference is not assigned!");
        }
    }

    public void CollectItem()
    {
        collectCount++;
        UpdateCountDisplay();
        PlayCollectSound();
        Debug.Log($"Item collected! Total: {collectCount}");
        if (collectCount >= 1)
        {
            FinalDoorCutscene();
        }
    }

    private void PlayCollectSound()
    {
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound, soundVolume);
        }
    }

    private void UpdateCountDisplay()
    {
        if (collectCountText != null)
        {
            collectCountText.text = collectCount.ToString() + "/10";
        }
    }

    public int GetCollectCount()
    {
        return collectCount;
    }

    public void ResetCount()
    {
        collectCount = 0;
        UpdateCountDisplay();
    }
    public void OpenFinalDoor()
    {
        if (collectCount >= 10)
        {
            FinalDoor.SetActive(true);
            Debug.Log("Final door opened!");
        }
        else
        {
            Debug.LogWarning("FinalDoor reference is not assigned!");
        }
    }
}