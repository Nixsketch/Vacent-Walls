using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Simple MusicManager that blends multiple stems based on a 0..1 intensity
// Each stem corresponds to an exposed AudioMixer parameter (in dB). Use AnimationCurves
// to control how each stem fades in as intensity increases.
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Stem Parameters")]
    // Names of exposed parameters in the AudioMixer (e.g. "Stem1Vol", "Stem2Vol")
    public string[] stemParameterNames;
    // Curves map overall intensity (0..1) to per-stem normalized volume (0..1).
    public AnimationCurve[] stemCurves;

    [Header("Smoothing")]
    public float rampSpeed = 1f; // how fast current intensity moves toward target

    float targetIntensity = 0f;
    float currentIntensity = 0f;

    [Header("Enemy Reporting")]
    public float maxReactionDistance = 30f; // how far enemies influence music
    public float reportTimeout = 2f; // seconds before an enemy report expires

    class EnemyReport
    {
        public float alertness;
        public Vector3 position;
        public bool detected;
        public float time;
    }

    private readonly Dictionary<int, EnemyReport> reports = new Dictionary<int, EnemyReport>();

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Basic sanity: if curves missing, make default linear curves
        if (stemParameterNames != null)
        {
            if (stemCurves == null || stemCurves.Length != stemParameterNames.Length)
            {
                stemCurves = new AnimationCurve[stemParameterNames.Length];
                for (int i = 0; i < stemCurves.Length; i++)
                    stemCurves[i] = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            }
        }
    }

    void Update()
    {
        // purge expired reports
        float now = Time.time;
        var keys = new List<int>(reports.Keys);
        foreach (var k in keys)
        {
            if (now - reports[k].time > reportTimeout)
                reports.Remove(k);
        }

        // compute intensity from enemy reports (reports may override manual targetIntensity)
        float reportIntensity = 0f;
        foreach (var kv in reports)
        {
            var r = kv.Value;
            if (r.detected)
            {
                reportIntensity = 1f;
                break;
            }
            float dist = Vector3.Distance(r.position, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
            float distFactor = 1f - Mathf.Clamp01(dist / maxReactionDistance);
            float candidate = r.alertness * distFactor;
            reportIntensity = Mathf.Max(reportIntensity, candidate);
        }

        // choose the greater of manual targetIntensity and reportIntensity
        float effectiveTarget = Mathf.Max(targetIntensity, reportIntensity);
        currentIntensity = Mathf.MoveTowards(currentIntensity, effectiveTarget, rampSpeed * Time.deltaTime);
        ApplyIntensityToMixer(currentIntensity);
    }

    // Public API
    public void SetIntensity(float v) => targetIntensity = Mathf.Clamp01(v);
    public void StartChase() => SetIntensity(1f);
    public void StopChase() => SetIntensity(0f);

    // Called by enemies to report alertness (0..1) and detection state. Reporter identity is used to track and timeout reports.
    public void ReportEnemyAlert(MonoBehaviour reporter, float alertness, Vector3 position, bool detected)
    {
        if (reporter == null) return;
        int id = reporter.GetInstanceID();
        EnemyReport r;
        if (!reports.TryGetValue(id, out r))
        {
            r = new EnemyReport();
            reports[id] = r;
        }
        r.alertness = Mathf.Clamp01(alertness);
        r.position = position;
        r.detected = detected;
        r.time = Time.time;
    }

    public void RemoveEnemyReport(MonoBehaviour reporter)
    {
        if (reporter == null) return;
        reports.Remove(reporter.GetInstanceID());
    }

    // Optionally call with a small hysteresis/time to avoid twitching
    void ApplyIntensityToMixer(float intensity)
    {
        if (audioMixer == null || stemParameterNames == null) return;

        for (int i = 0; i < stemParameterNames.Length; i++)
        {
            string param = stemParameterNames[i];
            AnimationCurve curve = (i < stemCurves.Length && stemCurves[i] != null) ? stemCurves[i] : AnimationCurve.Linear(0f, 0f, 1f, 1f);
            float norm = Mathf.Clamp01(curve.Evaluate(intensity)); // 0..1

            // Convert normalized volume to dB for mixer. -80 dB is effectively silent.
            float dB = Mathf.Lerp(-80f, 0f, norm);
            audioMixer.SetFloat(param, dB);
        }
    }
}
