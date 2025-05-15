using UnityEngine;
using UnityEngine.AI;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] float waitTimeOnWayPoint = 1f;
    [SerializeField] WaypointPath path;

    private NavMeshAgent agent;
    Animator animator;

    float time = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (path != null && agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(path.GetNextWaypoint());
        }
    }

    private void Update()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            if (agent.remainingDistance < 0.1f)
            {
                time += Time.deltaTime;
                if (time >= waitTimeOnWayPoint && path != null)
                {
                    time = 0f;
                    agent.destination = path.GetNextWaypoint();
                }
            }

            float normalizedSpeed = Mathf.InverseLerp(0, agent.speed, agent.velocity.magnitude);
            if (animator != null)
            {
                animator.SetFloat("Speed", normalizedSpeed);
            }
        }
    }

    public void DisableAgent()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.enabled = false;
        }
    }
}