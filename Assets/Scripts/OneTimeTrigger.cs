using UnityEngine;

public class DisappearOnLookAndWalkAway : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The player object or camera transform to calculate distance from.")]
    public Transform playerTransform;

    [Tooltip("How far away the player must walk before the text can be destroyed.")]
    public float maxDistanceThreshold = 8f;

    private Camera mainCamera;
    private Renderer textRenderer;
    private bool hasBeenSeen = false;

    void Start()
    {
        mainCamera = Camera.main;
        textRenderer = GetComponent<Renderer>();

        // Automatically target the main camera's transform if no player transform is set
        if (playerTransform == null && mainCamera != null)
        {
            playerTransform = mainCamera.transform;
        }
    }

    void Update()
    {
        if (mainCamera == null || textRenderer == null || playerTransform == null) return;

        // 1. Check if the text is currently inside the camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        bool isCurrentlyInView = GeometryUtility.TestPlanesAABB(planes, textRenderer.bounds);

        // 2. Track if the player has at least seen the text once
        if (isCurrentlyInView && !hasBeenSeen)
        {
            hasBeenSeen = true;
        }

        // 3. Calculate current distance between player and text
        float currentDistance = Vector3.Distance(transform.position, playerTransform.position);

        // 4. Destroy ONLY if it HAS been seen AND the player is looking away AND the player is far away
        if (hasBeenSeen && !isCurrentlyInView && currentDistance >= maxDistanceThreshold)
        {
            Destroy(gameObject);
        }
    }
}