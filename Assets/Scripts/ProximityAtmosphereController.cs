using UnityEngine;
using UnityEngine.Audio;

public class ProximityAtmosphereController : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    public AudioMixer mixer;               // assign mixer that has exposed parameter
    public string parameterName = "Atmos";
    public float minDistance = 1f;         // distance at which atmosphere is max
    public float maxDistance = 20f;        // distance at which atmosphere is min
    public float minValue = -80f;          // e.g., dB or parameter low
    public float maxValue = 0f;            // e.g., dB or parameter high
    public float smoothTime = 0.1f;

    [Header("Debug")]
    public bool logDebug = false;
    private float currentValue;
    private float valueVelocity;
    public AudioSource targetAudioSource;


    void Start()
    {
        if (targetAudioSource == null)
        {
            targetAudioSource = GetComponent<AudioSource>();
        }

        if (targetAudioSource != null)
        {
            // 2. MUTE THE MIXER FIRST before triggering playback!
            currentValue = minValue; // -80f
            if (mixer != null)
            {
                mixer.SetFloat(parameterName, minValue);
            }

            // 3. Now start the loop silently
            targetAudioSource.loop = true;
            if (!targetAudioSource.isPlaying)
            {
                targetAudioSource.Play();
            }
        }
    }

    void Update()
    {
        // Try to fall back to main camera as player if not set
        if (player == null)
        {
            if (Camera.main != null)
            {
                player = Camera.main.transform;
            }
            else
            {
                if (logDebug) Debug.LogWarning("ProximityAtmosphereController: player not assigned and Camera.main is null");
                return;
            }
        }

        if (enemy == null || mixer == null)
        {
            if (logDebug) Debug.LogWarning("ProximityAtmosphereController: enemy or mixer not assigned");
            return;
        }

        // Distance & Volume calculation
        float dist = Vector3.Distance(player.position, enemy.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist); // 0 when close, 1 when far
        float target = Mathf.Lerp(maxValue, minValue, t);            // close = maxValue (0dB), far = minValue (-80dB)

        currentValue = Mathf.SmoothDamp(currentValue, target, ref valueVelocity, smoothTime);
        currentValue = Mathf.Clamp(currentValue, Mathf.Min(minValue, maxValue), Mathf.Max(minValue, maxValue));

        bool ok = mixer.SetFloat(parameterName, currentValue);
        if (!ok && logDebug)
        {
            Debug.LogWarning($"ProximityAtmosphereController: AudioMixer.SetFloat failed — parameter '{parameterName}' not found on mixer '{mixer.name}'.");
        }

        // Detect if another manager might be overwriting the same parameter
        if (logDebug)
        {
            var mm = FindFirstObjectByType<MusicManager>();
            if (mm != null && mm.stemParameterNames != null)
            {
                for (int i = 0; i < mm.stemParameterNames.Length; i++)
                {
                    if (mm.stemParameterNames[i] == parameterName)
                    {
                        Debug.LogWarning($"ProximityAtmosphereController: parameter '{parameterName}' is also controlled by MusicManager (stem index {i}). This will cause conflicts.");
                        break;
                    }
                }
            }
        }
    }
}