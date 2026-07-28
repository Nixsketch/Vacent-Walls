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

    float currentValue;
    float valueVelocity;

    void Start()
    {
            AudioSource src = GetComponent<AudioSource>();

            if (src.clip == null) Debug.LogError("AudioSource: No Clip assigned!");
            if (src.pitch == 0) Debug.LogError("AudioSource: Pitch is 0!");
            if (!src.enabled) Debug.LogError("AudioSource: Component is disabled!");
            if (!gameObject.activeInHierarchy) Debug.LogError("AudioSource: GameObject is inactive!");

            // Check for Listener
            if (FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 0)
            {
                Debug.LogError("AudioSource: No AudioListener in scene!");
            }

            src.Play();
        if (player == null)
        {
            if (Camera.main != null) player = Camera.main.transform;
            else
            {
                if (logDebug) Debug.LogWarning("ProximityAtmosphereController: player not assigned and Camera.main is null");
            }
        }
        if (enemy == null)
        {
            if (logDebug) Debug.LogWarning("ProximityAtmosphereController: enemy not assigned");
        }
        if (mixer == null)
        {
            if (logDebug) Debug.LogWarning("ProximityAtmosphereController: mixer not assigned");
        }
        if ("Atmos" == parameterName && mixer != null)
        {
            // Check if the parameter exists in the mixer
            float testValue;
            bool ok = mixer.GetFloat(parameterName, out testValue);
            if (!ok && logDebug)
            {
                Debug.LogWarning($"ProximityAtmosphereController: AudioMixer parameter '{parameterName}' not found on mixer '{mixer.name}'.");
            }

            if ("Atmos" == parameterName && minValue == -80f && logDebug)
            {
                Debug.LogWarning("ProximityAtmosphereController: parameterName is set to default 'Atmos' and minValue is -80. This may result in silence when close to the enemy.");
            }
        }
    }

    private void Awake()
    {
        void Start() {
    AudioSource src = GetComponent<AudioSource>();
    
    if (src.clip == null) Debug.LogError("AudioSource: No Clip assigned!");
    if (src.pitch == 0) Debug.LogError("AudioSource: Pitch is 0!");
    if (!src.enabled) Debug.LogError("AudioSource: Component is disabled!");
    if (!gameObject.activeInHierarchy) Debug.LogError("AudioSource: GameObject is inactive!");
    
    // Check for Listener
    if (FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 0) {
        Debug.LogError("AudioSource: No AudioListener in scene!");
    }

    src.Play();
}   
    }

    void Update()
    {
        // try to fall back to main camera as player if not set
        if (player == null)
        {
            if (Camera.main != null) player = Camera.main.transform;
            else
            {
                if (logDebug) Debug.LogWarning("ProximityAtmosphereController: player not assigned and Camera.main is null");
                return;
            }
        }

        if (enemy == null || mixer == null) {
            if (logDebug) Debug.LogWarning("ProximityAtmosphereController: enemy or mixer not assigned");
            return;
        }

        float dist = Vector3.Distance(player.position, enemy.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, dist); // 0 when very close, 1 when far
        float target = Mathf.Lerp(maxValue, minValue, t); // closer => higher (maxValue)
        currentValue = Mathf.SmoothDamp(currentValue, target, ref valueVelocity, smoothTime);

        // clamp in case of numerical issues
        currentValue = Mathf.Clamp(currentValue, Mathf.Min(minValue, maxValue), Mathf.Max(minValue, maxValue));

        bool ok = mixer.SetFloat(parameterName, currentValue);
        if (!ok && logDebug)
        {
            Debug.LogWarning($"ProximityAtmosphereController: AudioMixer.SetFloat failed — parameter '{parameterName}' not found on mixer '{(mixer != null ? mixer.name : "null")}'.");
        }

        // Detect if another manager might be overwriting the same parameter
        if (logDebug)
        {
            var mm = FindObjectOfType<MusicManager>();
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