using UnityEngine;

[RequireComponent(typeof(EnemyMove))]
public class EnemyAudioReporter : MonoBehaviour
{
    [Header("Alert tuning")]
    public float hearingAlertLevel = 0.6f; // how loud noises raise alertness
    public float investigateAlertLevel = 0.6f; // alertness when investigating
    public float decayRate = 0.5f; // per second
    public float minAlertForReport = 0.02f; // don't spam reports under this
    [Header("Hearing")]
    [Tooltip("Eye height used for occlusion raycasts when evaluating noise attenuation")]
    public float eyeHeight = 1.6f;
    [Tooltip("If true, log debug info when noises are processed")]
    public bool logDebug = false;

    private float alertness = 0f;
    private EnemyMove enemyMove;
    private Transform player;

    void Awake()
    {
        enemyMove = GetComponent<EnemyMove>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void OnEnable()
    {
        NoiseManager.OnNoiseEmitted += OnNoise;
    }

    void OnDisable()
    {
        NoiseManager.OnNoiseEmitted -= OnNoise;
        if (MusicManager.Instance != null)
            MusicManager.Instance.RemoveEnemyReport(this);
    }

    void OnNoise(Vector3 pos, float strength, GameObject source)
    {
        if (source == this.gameObject) return;

        float dist = Vector3.Distance(transform.position, pos);
        if (dist > strength) return; // out of hearing range (quick reject)

        // partial occlusion: attenuate strength for each blocking collider between source and this enemy
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 dir = pos - origin;
        float effectiveStrength = strength;

        RaycastHit[] hits = Physics.RaycastAll(origin, dir.normalized, dir.magnitude, ~0, QueryTriggerInteraction.Ignore);
        int blockerCount = 0;
        foreach (var h in hits)
        {
            // ignore hits on the source, player or self
            if (h.transform == source || h.transform == player || h.transform == transform) continue;
            if (source != null && h.transform.IsChildOf(source.transform)) continue;
            if (player != null && h.transform.IsChildOf(player)) continue;
            // count as a blocker
            blockerCount++;
        }

        if (blockerCount > 0)
        {
            // attenuate exponentially: each blocker halves the audible strength
            effectiveStrength *= Mathf.Pow(0.5f, blockerCount);
        }

        if (dist > effectiveStrength) return; // after attenuation, out of hearing range

        // heard it: boost alertness proportionally to effective strength and distance
        float factor = Mathf.Clamp01(1f - (dist / Mathf.Max(0.0001f, effectiveStrength)));
        float boost = hearingAlertLevel * factor;
        alertness = Mathf.Max(alertness, boost);

        if (logDebug)
        {
            Debug.LogFormat(this, "EnemyAudioReporter.OnNoise: pos={0} rawStrength={1:F2} blockers={2} effective={3:F2} dist={4:F2} boost={5:F2} alertness={6:F2}",
                pos, strength, blockerCount, effectiveStrength, dist, boost, alertness);
        }
    }

    void Update()
    {
        bool reportedDetected = false;

        if (enemyMove != null)
        {
            if (enemyMove.IsChasing)
            {
                alertness = 1f;
                reportedDetected = true;
            }
            else if (enemyMove.IsInvestigating)
            {
                alertness = Mathf.Max(alertness, investigateAlertLevel);
            }
        }

        // decay over time
        alertness = Mathf.MoveTowards(alertness, 0f, decayRate * Time.deltaTime);

        if (alertness >= minAlertForReport)
        {
            if (MusicManager.Instance != null)
                MusicManager.Instance.ReportEnemyAlert(this, alertness, transform.position, reportedDetected);
        }
        else
        {
            if (MusicManager.Instance != null)
                MusicManager.Instance.RemoveEnemyReport(this);
        }
    }
}
