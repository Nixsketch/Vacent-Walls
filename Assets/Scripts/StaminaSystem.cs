using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainRate = 25f;
    [SerializeField] private float regenRate = 15f;
    [SerializeField] private float regenDelay = 1.2f;

    [Header("UI Elements")]
    [SerializeField] private UnityEngine.UI.Image staminaBarFill;

    public float CurrentStamina { get; private set; }

    private float regenTimer;

    private void Start()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        UpdateUI();
    }

    // Call this from PlayerMovement when sprinting
    public bool TrySprint(float deltaTime)
    {
        if (CurrentStamina > 0)
        {
            CurrentStamina -= drainRate * deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
            regenTimer = 0f; // Reset regen delay while sprinting
            return true;
        }

        return false; // Out of stamina
    }

    // Call this from PlayerMovement when NOT sprinting
    public void RegenerateStamina(float deltaTime)
    {
        if (CurrentStamina < maxStamina)
        {
            regenTimer += deltaTime;

            if (regenTimer >= regenDelay)
            {
                CurrentStamina += regenRate * deltaTime;
                CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
            }
        }
    }

    private void UpdateUI()
    {
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = CurrentStamina / maxStamina;
        }
    }
}