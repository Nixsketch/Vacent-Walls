using UnityEngine;

public class Collectable : MonoBehaviour
{
    private CollectSystem collectSystem;

    private void Start()
    {
        // Find the CollectSystem in the scene
        collectSystem = FindAnyObjectByType<CollectSystem>();
        if (collectSystem == null)
        {
            Debug.LogError("CollectSystem not found in scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched this collectible
        if (other.CompareTag("Player"))
        {
            // Notify the collect system
            if (collectSystem != null)
            {
                collectSystem.CollectItem();
            }

            // Destroy the collectible
            Destroy(gameObject);
        }
    }
}
