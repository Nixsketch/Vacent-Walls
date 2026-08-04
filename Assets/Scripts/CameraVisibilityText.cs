using UnityEngine;

public class CameraVisibilityText : MonoBehaviour
{
    private Camera mainCamera;
    private Renderer textRenderer;

    void Start()
    {
        // Cache the main camera and the renderer of the text object
        mainCamera = Camera.main;
        textRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (mainCamera == null || textRenderer == null) return;

        // Calculate the camera's view frustum planes
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        // Check if the text's bounding box is inside the camera planes
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, textRenderer.bounds);

        // Toggle visibility
        textRenderer.enabled = isVisible;
    }
}