using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaBarImageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private Image staminaBackgroundImage; // Assign your Background Image here
    [SerializeField] private CanvasGroup staminaCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float staminaMax = 100f;
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private Color normalColor = Color.gray; // Or your original background color
    [SerializeField] private Color emptyColor = Color.red;
    [SerializeField] private float blinkSpeed = 0.15f; // Seconds per blink

    private float currentStamina;
    private bool isSprinting;
    private float targetAlpha;

    // Coroutine reference to stop it when stamina recovers
    private Coroutine blinkCoroutine;

    public void SetSprinting(bool sprinting, float currentStaminaValue)
    {
        isSprinting = sprinting;
        currentStamina = currentStaminaValue;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // 1. Update Fill (Normal 0-100% logic, no 50% trick)
        staminaFillImage.fillAmount = Mathf.Clamp01(currentStamina / staminaMax);

        // 2. Handle Visibility
        bool isFull = currentStamina >= staminaMax;

        // Visible if sprinting OR (not sprinting but not full/regenerating)
        if (isFull && !isSprinting)
            targetAlpha = 0f;
        else
            targetAlpha = 1f;

        staminaCanvasGroup.alpha = Mathf.Lerp(staminaCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        if (staminaCanvasGroup.alpha < 0.01f) staminaCanvasGroup.alpha = 0f;

        // 3. Handle Red Blink Logic
        if (currentStamina <= 0f && isSprinting) // Only blink if empty AND trying to sprint
        {
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkBackground());
            }
        }
        else
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                staminaBackgroundImage.color = normalColor; // Reset color
            }
        }
    }

    private IEnumerator BlinkBackground()
    {
        while (true)
        {
            staminaBackgroundImage.color = emptyColor; // Red
            yield return new WaitForSeconds(blinkSpeed);

            // Optional: Fade out completely or just to normal color? 
            // Usually flashing Red -> Normal Color looks best.
            staminaBackgroundImage.color = normalColor;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}