using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    public float maxBatteryLife = 100f;
    public float batteryDrainRate = 2f;
    public Image batterydrainfill;
    public AudioSource batterydrainsound;


    void Start()
    {
        flashlight.enabled = false;
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
            }
        }
    }
}