using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    public Transform[] waypoints;

    [Header("Speeds")]
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 5.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] [Range(0f, 180f)] private float fieldOfView = 120f;
    [SerializeField] private float eyeHeight = 1.6f;

    [Header("Chase / Search")]
    [SerializeField] private float chaseTimeAfterLost = 1.2f;
    [SerializeField] private float investigateSearchDuration = 4f;
    [SerializeField] private float searchRadius = 3f;

    [Header("Animator (optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animSpeedParam = "Speed";
    [SerializeField] private string animChaseParam = "IsChasing";

    private NavMeshAgent agent;
    private Transform player;
    private int currentWaypoint = 0;
    private float lastSeenTime = -Mathf.Infinity;
    private Vector3 lastSeenPosition = Vector3.zero;

    private float searchTimer = 0f;

    private enum State { Patrol, Chase, Investigate, Search }
    private State state = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent == null)
            Debug.LogError("EnemyMove requires a NavMeshAgent component.");

        if (player == null)
            Debug.LogWarning("EnemyMove: No GameObject with tag 'Player' found. Chasing will be disabled until a player is assigned.");

        // sensible defaults for NavMeshAgent
        agent.autoBraking = true;
        agent.updateRotation = true;
        agent.speed = patrolSpeed;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 1.2f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;

        // ensure agent starts on the NavMesh to avoid being stuck off-mesh
        SnapToNavMesh();

        if (waypoints != null && waypoints.Length > 0)
            SafeSetDestination(waypoints[0].position);
        else
            SafeSetDestination(transform.position);
    }

    void Update()
    {
        bool canSee = false;
        if (player != null)
        {
            Vector3 toPlayer = player.position - transform.position;
            float sqrDist = toPlayer.sqrMagnitude;

            if (sqrDist <= detectionRange * detectionRange)
            {
                float angle = Vector3.Angle(transform.forward, toPlayer);
                if (angle <= fieldOfView * 0.5f)
                {
                    Vector3 origin = transform.position + Vector3.up * eyeHeight;
                    Vector3 target = player.position + Vector3.up * 0.9f;
                    Vector3 dir = target - origin;
                    RaycastHit hit;
                    if (Physics.Raycast(origin, dir.normalized, out hit, Mathf.Sqrt(sqrDist), ~0, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.transform == player || hit.transform.IsChildOf(player))
                            canSee = true;
                    }
                }
            }
        }

        // update animator
        if (animator != null)
        {
            animator.SetFloat(animSpeedParam, agent.velocity.magnitude);
            animator.SetBool(animChaseParam, state == State.Chase);
        }

        if (canSee)
        {
            lastSeenTime = Time.time;
            lastSeenPosition = player.position;
            if (state != State.Chase)
            {
                state = State.Chase;
                agent.speed = chaseSpeed;
            }
        }

        switch (state)
        {
            case State.Chase:
            if (player != null)
                SafeSetDestination(player.position);

                if (Time.time - lastSeenTime > chaseTimeAfterLost)
                {
                    // lost sight: go investigate last known position
                    state = State.Investigate;
                    agent.speed = patrolSpeed;
                    SafeSetDestination(lastSeenPosition);
                }
                break;

            case State.Investigate:
                // move to last seen position; when arrive, start searching
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    state = State.Search;
                    searchTimer = investigateSearchDuration;
                }
                break;

            case State.Search:
                // simple search: rotate and occasionally move to a random nearby point
                searchTimer -= Time.deltaTime;
                if (searchTimer <= 0f)
                {
                    // give up, return to patrol
                    state = State.Patrol;
                    GoToNearestWaypoint();
                }
                else
                {
                    // pick a random point inside radius occasionally
                    if (!agent.hasPath || agent.remainingDistance < 0.5f)
                    {
                        Vector3 randomPoint = lastSeenPosition + Random.insideUnitSphere * searchRadius;
                        randomPoint.y = transform.position.y;
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(randomPoint, out hit, 1.5f, NavMesh.AllAreas))
                        {
                            SafeSetDestination(hit.position);
                        }
                    }
                }
                break;

            case State.Patrol:
                if (waypoints == null || waypoints.Length == 0)
                    return;

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                    SafeSetDestination(waypoints[currentWaypoint].position);
                }
                break;
        }
    }

    private void GoToNearestWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        int nearest = 0;
        float bestSqr = float.PositiveInfinity;
        for (int i = 0; i < waypoints.Length; i++)
        {
            float sqr = (waypoints[i].position - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                nearest = i;
            }
        }

        currentWaypoint = nearest;
        SafeSetDestination(waypoints[currentWaypoint].position);
    }

    // Ensure the agent is placed on the NavMesh at start
    private void SnapToNavMesh()
    {
        if (agent == null) return;
        NavMeshHit hit;
        float sampleDistance = 2f;
        if (NavMesh.SamplePosition(transform.position, out hit, sampleDistance, NavMesh.AllAreas))
        {
            // warp keeps the agent internal state consistent
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarningFormat(this, "EnemyMove: No NavMesh near spawn for '{0}' at {1}. Agent may be off-mesh.", name, transform.position);
        }
    }

    // Safely set destination with validation and fallback
    private void SafeSetDestination(Vector3 target)
    {
        if (agent == null) return;

        if (float.IsNaN(target.x) || float.IsNaN(target.y) || float.IsNaN(target.z) ||
            float.IsInfinity(target.x) || float.IsInfinity(target.y) || float.IsInfinity(target.z))
        {
            Debug.LogErrorFormat(this, "EnemyMove: Attempted to set invalid destination {0}", target);
            return;
        }

        // if target is off the NavMesh, sample nearby point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // try sampling from the agent's position towards the target
            Vector3 dir = (target - transform.position).normalized;
            Vector3 probe = transform.position + dir * 1.0f;
            if (NavMesh.SamplePosition(probe, out hit, 1.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogWarningFormat(this, "EnemyMove: Could not find NavMesh position near target {0} for '{1}'", target, name);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // draw fov lines
        Vector3 left = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);

        // draw last seen position and search radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastSeenPosition, 0.25f);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawWireSphere(lastSeenPosition, searchRadius);
    }
}
