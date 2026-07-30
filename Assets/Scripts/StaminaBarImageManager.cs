using UnityEngine;
using UnityEngine.UI;

public class StaminaBarImageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private CanvasGroup staminaCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float staminaMax = 100f;
    [SerializeField] private float fadeSpeed = 10f;

    private float currentStamina;
    private bool isSprinting;
    private float targetAlpha;

    // Updated to handle state and value simultaneously
    public void SetSprinting(bool sprinting, float currentStaminaValue)
    {
        isSprinting = sprinting;
        currentStamina = currentStaminaValue;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // 1. Calculate Fill Amount
        float fillPercent = currentStamina / staminaMax;

        // FIX 1: Only show 50% if stamina is TRULY empty (0). 
        // If it's 0.1, 1.0, or anything > 0, show the actual percentage.
        // We use a very small threshold to avoid floating point jitter.
        if (currentStamina <= 0f)
        {
            staminaFillImage.fillAmount = 0.5f; // Show halfway only when completely empty
        }
        else
        {
            staminaFillImage.fillAmount = fillPercent;
        }

        // 2. Calculate Visibility (Alpha)
        // FIX 2: The bar should be visible if:
        // A) We are currently sprinting, OR
        // B) We are NOT sprinting BUT stamina is not full (regenerating).
        // It should ONLY disappear if: Not Sprinting AND Stamina is Full.

        bool isFull = currentStamina >= staminaMax;

        if (isFull && !isSprinting)
        {
            // Only hide if fully regenerated AND not sprinting
            targetAlpha = 0f;
        }
        else
        {
            // Show if sprinting OR if regenerating (not full)
            targetAlpha = 1f;
        }

        // 3. Apply Smooth Fade
        staminaCanvasGroup.alpha = Mathf.Lerp(staminaCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

        // Optimization: Snap to 0 if very close to avoid rendering invisible objects
        if (staminaCanvasGroup.alpha < 0.01f) staminaCanvasGroup.alpha = 0f;
    }
}