using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NoiseEmitter : MonoBehaviour
{
    [Header("Noise")]
    public float baseStrength = 3f;

    [Header("SFX")]
    public AudioClip sfxClip;
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
        if (playSfx && sfxClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(sfxClip, sfxVolume);
        }
    }

    // Emit noise at an explicit position (useful for thrown objects)
    public void EmitNoiseAt(Vector3 position, float strengthMultiplier = 1f, bool playSfx = true)
    {
        float strength = baseStrength * strengthMultiplier;
        NoiseManager.EmitNoise(position, strength, gameObject);
        if (playSfx && sfxClip != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(sfxClip, position, sfxVolume);
        }
    }
}
