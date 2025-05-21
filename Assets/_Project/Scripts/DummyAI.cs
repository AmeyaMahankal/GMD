using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyPathing))]
public class DummyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement / Combat")]
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private int baseDamageToPlayer = 5;
    [SerializeField] private float attackCooldown = 5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float fovAngle = 45f;

    [Header("Stealth-Kill")]
    [SerializeField] private float killDistance = 2f;
    [SerializeField] private float killAngle = 45f;

    private enum AIState { Patrol, Chase, Attack }
    private AIState state = AIState.Patrol;

    private NavMeshAgent agent;
    private EnemyPathing patrol;
    private PlayerScript playerScript;

    private float cooldown;
    private float lostTimer;
    private const float LOST_SIGHT_GRACE = 3f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<EnemyPathing>();
        CachePlayerReference();
    }

    private void Start()
    {
        cooldown = attackCooldown;
    }

    private void Update()
    {
        if (playerScript == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // NEW: Smarter detection based on crouch status
        bool inSight = false;

        if (playerScript.IsStealthed)
        {
            inSight = distance <= detectionRange &&
                      InFOV(transform, player, fovAngle, detectionRange);
        }
        else
        {
            inSight = distance <= detectionRange;
        }

        switch (state)
        {
            case AIState.Patrol:
                if (inSight)
                {
                    patrol.StopPatrol();
                    agent.isStopped = false;
                    state = AIState.Chase;
                }
                break;

            case AIState.Chase:
                if (inSight)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    lostTimer = LOST_SIGHT_GRACE;

                    RotateTowardPlayer();

                    if (distance <= stopDistance)
                    {
                        state = AIState.Attack;
                    }
                }
                else
                {
                    lostTimer -= Time.deltaTime;

                    if (lostTimer > 0f)
                    {
                        if (!agent.pathPending && agent.remainingDistance < 0.2f)
                        {
                            Vector3 searchOffset = Random.insideUnitSphere * 2f;
                            searchOffset.y = 0;
                            Vector3 searchPosition = player.position + searchOffset;

                            if (NavMesh.SamplePosition(searchPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                            {
                                agent.SetDestination(hit.position);
                            }
                        }
                    }
                    else
                    {
                        patrol.StartPatrol();
                        state = AIState.Patrol;
                    }
                }
                break;

            case AIState.Attack:
                agent.isStopped = true;
                cooldown -= Time.deltaTime;

                RotateTowardPlayer();

                if (cooldown <= 0f)
                {
                    int dmg = playerScript.isBlocking ? 2 : baseDamageToPlayer;
                    playerScript.TakeDamage(dmg);
                    cooldown = attackCooldown;
                }

                if (distance > stopDistance)
                {
                    cooldown = attackCooldown;
                    agent.isStopped = false;
                    state = AIState.Chase;
                }
                break;
        }
    }

    private void RotateTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void CachePlayerReference()
    {
        if (player != null)
            playerScript = player.GetComponent<PlayerScript>();
        else
        {
            playerScript = FindObjectOfType<PlayerScript>();
            if (playerScript != null)
                player = playerScript.transform;
        }
    }

    public static bool InFOV(Transform origin, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(origin.position, maxRadius, overlaps);

        for (int i = 0; i < count; i++)
        {
            if (overlaps[i] && overlaps[i].transform == target)
            {
                Vector3 dir = (target.position - origin.position).normalized;
                dir.y = 0;

                if (Vector3.Angle(origin.forward, dir) > maxAngle) continue;

                if (Physics.Raycast(origin.position, target.position - origin.position,
                                    out RaycastHit hit, maxRadius) &&
                    hit.transform == target)
                    return true;
            }
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 left = Quaternion.AngleAxis(fovAngle, transform.up) * transform.forward * detectionRange;
        Vector3 right = Quaternion.AngleAxis(-fovAngle, transform.up) * transform.forward * detectionRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, left);
        Gizmos.DrawRay(transform.position, right);

        if (player != null)
        {
            bool inside = InFOV(transform, player, fovAngle, detectionRange);
            Gizmos.color = inside ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position,
                           (player.position - transform.position).normalized * detectionRange);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killDistance);

        Vector3 killL = Quaternion.AngleAxis(killAngle, transform.up) * -transform.forward * killDistance;
        Vector3 killR = Quaternion.AngleAxis(-killAngle, transform.up) * -transform.forward * killDistance;
        Gizmos.color = new Color(1f, 0f, 1f);
        Gizmos.DrawRay(transform.position, killL);
        Gizmos.DrawRay(transform.position, killR);
    }
#endif
}
