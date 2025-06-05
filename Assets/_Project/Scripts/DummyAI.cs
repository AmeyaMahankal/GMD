using UnityEngine;
using UnityEngine.AI;
using _Project.Scripts.PlayerScript.Interfaces;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyPathing), typeof(PlayerDetection))]
public class DummyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement / Combat")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private float attackCooldown = 3f;

    private enum AIState { Patrol, Chase, Attack }
    private AIState state = AIState.Patrol;

    private NavMeshAgent agent;
    private EnemyPathing patrol;
    private Animator animator;
    private PlayerScript playerScript;
    private PlayerStealth stealth;
    private PlayerDetection playerDetection;
    private EnemyCombat enemyCombat;

    private float cooldown;
    private float lostTimer;
    private const float LOST_SIGHT_GRACE = 3f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<EnemyPathing>();
        playerDetection = GetComponent<PlayerDetection>();
        enemyCombat = GetComponent<EnemyCombat>();
        animator = GetComponent<Animator>();

        CachePlayerReference();
        agent.speed = speed;
    }

    private void Start()
    {
        cooldown = 0f;
    }

    private void Update()
    {
        if (playerScript == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inSight = playerDetection.CanSeePlayer();

        switch (state)
        {
            case AIState.Patrol:
                SetAnimSpeed(0f);

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

                    Vector3 dir = (player.position - transform.position).normalized;
                    Vector3 targetPos = player.position - dir * stopDistance;

                    if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }

                    lostTimer = LOST_SIGHT_GRACE;
                    RotateTowardPlayer();
                    SetAnimSpeed(agent.velocity.magnitude);

                    if (distance <= stopDistance)
                    {
                        agent.isStopped = true;
                        state = AIState.Attack;
                        cooldown = 0f;
                    }
                }
                else
                {
                    lostTimer -= Time.deltaTime;
                    SetAnimSpeed(agent.velocity.magnitude);

                    if (lostTimer <= 0f)
                    {
                        patrol.StartPatrol();
                        state = AIState.Patrol;
                    }
                }
                break;

            case AIState.Attack:
                RotateTowardPlayer();
                SetAnimSpeed(0f);

                cooldown -= Time.deltaTime;

                if (cooldown <= 0f)
                {
                    if (animator != null) animator.SetTrigger("attack");
                    cooldown = attackCooldown;
                }

                if (distance > stopDistance + 0.5f)
                {
                    agent.isStopped = false;
                    state = AIState.Chase;
                }
                break;
        }
    }

    private void SetAnimSpeed(float speed)
    {
        if (animator != null)
            animator.SetFloat("speed", speed);
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
        {
            playerScript = player.GetComponent<PlayerScript>();
            stealth = player.GetComponent<PlayerStealth>();
        }
        else
        {
            playerScript = FindObjectOfType<PlayerScript>();
            if (playerScript != null)
            {
                player = playerScript.transform;
                stealth = playerScript.GetComponent<PlayerStealth>();
            }
        }

        if (playerDetection != null)
            playerDetection.player = player;

        if (enemyCombat != null)
            enemyCombat.SetPlayer(player);
    }
}
