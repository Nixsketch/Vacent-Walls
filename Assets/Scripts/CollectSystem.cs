using UnityEngine;
using TMPro;

public class CollectSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectCountText;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float soundVolume = 1f;

    private int collectCount = 0;
    private AudioSource audioSource;

    void Start()
    {
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
            }
        }
    }

    public void CollectItem()
    {
        collectCount++;
        UpdateCountDisplay();
        PlayCollectSound();
        Debug.Log($"Item collected! Total: {collectCount}");
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
            collectCountText.text = collectCount.ToString();
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
}
