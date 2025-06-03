using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathing : MonoBehaviour
{
    [SerializeField] float waitTimeOnWayPoint = 1f;
    [SerializeField] WaypointPath path;

    private NavMeshAgent agent;
    private Animator animator;
    private float time;

    // ─────────────────────────── ACCESS BY DummyAI ──
    public bool IsPatrolling => enabled;          // true while Update() is running
    public void StartPatrol()
    {
        enabled = true;                           // re-enable Update loop
        if (path != null && agent.isActiveAndEnabled)
            agent.SetDestination(path.GetClosestWaypoint(transform.position));
    }
    public void StopPatrol()
    {
        enabled = false;                          // freezes Update loop
        if (agent != null && agent.isActiveAndEnabled)
            agent.ResetPath();                    // clear residual waypoints
    }
    // ────────────────────────────────────────────────

    private void Awake()
    {
        agent    = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        if (path != null && agent.isActiveAndEnabled)
            agent.SetDestination(path.GetNextWaypoint());
    }
    private void Update()
    {
        if (!agent || !agent.isActiveAndEnabled) return;

        if (agent.remainingDistance < 0.1f)
        {
            time += Time.deltaTime;
            if (time >= waitTimeOnWayPoint && path != null)
            {
                time = 0f;
                agent.destination = path.GetNextWaypoint();
            }
        }

        if (animator)
        {
            float normSpeed = Mathf.InverseLerp(0, agent.speed, agent.velocity.magnitude);
            animator.SetFloat("speed", normSpeed);
        }
    }
}