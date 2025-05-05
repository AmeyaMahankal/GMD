using UnityEngine;
using UnityEngine.AI;

public class EnemyPathing : MonoBehaviour
{

    [SerializeField] float waitTimeOnWayPoint = 1f;
    [SerializeField] WaypointPath path;

    NavMeshAgent agent;
    Animator animator;

    float time = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

    }

    private void Start()
    {
       agent.SetDestination(path.GetNextWaypoint());
 
    }

    private void Update()
    {
        if (agent.remainingDistance < 0.1f)
        {
            time += Time.deltaTime;
            if (time >= waitTimeOnWayPoint)
            {
                time = 0f;
                agent.destination = path.GetNextWaypoint();
            }
        }

        float normalizedSpeed = Mathf.InverseLerp(0, agent.speed, agent.velocity.magnitude);
        animator.SetFloat("Speed", normalizedSpeed);
    }
}


