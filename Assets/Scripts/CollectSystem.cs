using UnityEngine;
using TMPro;

public class CollectSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectCountText;
    private int collectCount = 0;

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
    }

    public void CollectItem()
    {
        collectCount++;
        UpdateCountDisplay();
        Debug.Log($"Item collected! Total: {collectCount}");
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
