using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NoiseEmitter))]
public class EnemyFootsteps : MonoBehaviour
{
    public float stepIntervalPatrol = 0.6f; // Interval between footsteps while patrolling
    public float stepIntervalChase = 0.3f; // Interval between footsteps while chasing
    public float patrolNoiseMultiplier = 1.0f; // Noise multiplier for patrolling
    public float chaseNoiseMultiplier = 2.0f; // Noise multiplier for chasing
    public float moveSpeedThreshold = 0.1f; // Minimum speed to consider the enemy moving

    [Header("3D Spatial Audio")]
    public float maxAudiableDist = 50f; // How far away footsteps can be heard
    public float minDistance = 1f; // Distance at which audio starts to attenuate

    private NoiseEmitter noiseEmitter;
    private NavMeshAgent agent;
    private EnemyMove enemyMove;
    private AudioSource audioSource;
    private float lastStepTime = -10f;

    void Awake()
    {
        noiseEmitter = GetComponent<NoiseEmitter>();
        agent = GetComponent<NavMeshAgent>();
        enemyMove = GetComponent<EnemyMove>();
        audioSource = GetComponent<AudioSource>();

        // Enable 3D spatial audio
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; // 1 = full 3D audio
            audioSource.maxDistance = maxAudiableDist;
            audioSource.minDistance = minDistance;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // Natural distance falloff
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (noiseEmitter == null || agent == null || enemyMove == null)
            return;

        float speed = agent.velocity.magnitude;
        bool isMoving = speed > moveSpeedThreshold;

        if (!isMoving) return;

        bool isChasing = (enemyMove != null) && enemyMove.IsChasing;
        float interval = isChasing ? stepIntervalChase : stepIntervalPatrol;
        float multiplier = isChasing ? chaseNoiseMultiplier : patrolNoiseMultiplier;

        if (Time.time - lastStepTime >= interval)
        {
            noiseEmitter.EmitNoise(multiplier, true);
            lastStepTime = Time.time;
        }
    }
}
