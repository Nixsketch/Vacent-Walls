using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    public float maxBatteryLife = 100f;
    public float batteryDrainRate = 5f;
    public Image batterydrainfill;
    public AudioSource batterydrainsound;
    public Image flashlighton;
    public Image flashlightoff;


    void Start()
    {
        flashlight.enabled = false;
        flashlighton.enabled = false;
        flashlightoff.enabled = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
            batterydrainsound = GetComponent<AudioSource>();
            batterydrainsound.loop = false;
            batterydrainsound.volume = 1.0f;
            batterydrainsound.Play();

            if (flashlight.enabled) { maxBatteryLife -= batteryDrainRate * Time.deltaTime; }
            if (maxBatteryLife < 0)
            {
                maxBatteryLife = 0;
                flashlight.enabled = false;
                flashlighton.enabled = false;
                flashlightoff.enabled = true;
            }
            if (flashlight.enabled) {
                flashlighton.enabled = true;
                flashlightoff.enabled = false;
            }
            else
            {
                flashlight.enabled = false;
                flashlighton.enabled = false;
                flashlightoff.enabled = true;
            }
        }
    }
}