using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggle : MonoBehaviour
{
    [Header("References")]
    public Light flashlight;
    public Image batteryFill;
    public Image flashlightOnIcon;
    public Image flashlightOffIcon;
    public AudioSource toggleSound;

    [Header("Battery Settings")]
    public float maxBatteryLife = 100f;
    public float currentBatteryLife = 100f;
    public float drainRate = 6f;
    public float rechargeRate = 20f;

    [Header("Audio Settings")]
    public AudioClip clickSound;
    public AudioClip deadSound; // Optional: play a "dead" buzz when battery dies
    public AudioClip rechargeSound;

    private bool isOn = false;
    private bool isBatteryDead = false; // New state tracker

    void Start()
    {
        currentBatteryLife = maxBatteryLife;
        SetFlashlightState(false);
    }

    void Update()
    {
        // 1. Handle Toggle Input (Only if battery is not dead)
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }

        // 2. Handle Battery Logic
        if (isOn)
        {
            // Drain battery
            currentBatteryLife -= drainRate * Time.deltaTime;

            // Check for empty battery
            if (currentBatteryLife <= 0)
            {
                currentBatteryLife = 0;
                isBatteryDead = true; // Lock the toggle
                SetFlashlightState(false); // Force turn off
                toggleSound.PlayOneShot(deadSound); // Optional: Play dead sound
            }
        }
        else
        {
            // Recharge battery when off
            currentBatteryLife += rechargeRate * Time.deltaTime;

            // Cap at max
            if (currentBatteryLife >= maxBatteryLife)
            {
                currentBatteryLife = maxBatteryLife;
                isBatteryDead = false; // Unlock the toggle
            }
        }

        // 3. Update UI
        if (batteryFill != null)
        {
            batteryFill.fillAmount = currentBatteryLife / maxBatteryLife;
        }

        // Optional: Visual feedback for dead battery (e.g., change icon color)
        if (flashlightOffIcon != null && isBatteryDead)
        {
             flashlightOffIcon.color = Color.red; // Uncomment to turn red when dead
        }
        else { 
        flashlightOffIcon.color = Color.white;} // Uncomment to turn white when not dead
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        SetFlashlightState(isOn);
        toggleSound.PlayOneShot(clickSound);
    }

    void SetFlashlightState(bool state)
    {
        isOn = state;

        if (flashlight != null)
            flashlight.enabled = isOn;

        if (flashlightOnIcon != null)
            flashlightOnIcon.enabled = isOn;

        if (flashlightOffIcon != null)
            flashlightOffIcon.enabled = !isOn;
    }
}