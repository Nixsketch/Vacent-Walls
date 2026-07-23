using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NoiseEmitter : MonoBehaviour
{
    [Header("Noise")]
    [Tooltip("Default noise range/strength for emitted sounds. Walk = baseStrength × 1, Run = baseStrength × runNoiseMultiplier")]
    public float baseStrength = 8f;

    [Header("SFX")]
    // Single clip fallback (kept for backwards compatibility)
    public AudioClip sfxClip;
    // Multiple clips (e.g., footstep variants). If non-empty, a random clip will be chosen.
    public AudioClip[] sfxClips;
    [Range(0f,1f)] public float sfxVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    // Emit noise at this emitter's position
    public void EmitNoise(float strengthMultiplier = 1f, bool playSfx = true)
    {
        float strength = baseStrength * strengthMultiplier;
        NoiseManager.EmitNoise(transform.position, strength, gameObject);
        if (playSfx && audioSource != null)
        {
            AudioClip clipToPlay = null;
            if (sfxClips != null && sfxClips.Length > 0)
            {
                int idx = UnityEngine.Random.Range(0, sfxClips.Length);
                clipToPlay = sfxClips[idx];
            }
            else
            {
                clipToPlay = sfxClip;
            }

            if (clipToPlay != null)
                audioSource.PlayOneShot(clipToPlay, sfxVolume);
        }
    }

    // Emit noise at an explicit position (useful for thrown objects)
    public void EmitNoiseAt(Vector3 position, float strengthMultiplier = 1f, bool playSfx = true)
    {
        float strength = baseStrength * strengthMultiplier;
        NoiseManager.EmitNoise(position, strength, gameObject);
        if (playSfx)
        {
            AudioClip clipToPlay = null;
            if (sfxClips != null && sfxClips.Length > 0)
            {
                int idx = UnityEngine.Random.Range(0, sfxClips.Length);
                clipToPlay = sfxClips[idx];
            }
            else
            {
                clipToPlay = sfxClip;
            }

            if (clipToPlay != null)
                AudioSource.PlayClipAtPoint(clipToPlay, position, sfxVolume);
        }
    }
}
