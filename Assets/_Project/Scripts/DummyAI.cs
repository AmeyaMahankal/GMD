using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyPathing))]
public class DummyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Movement / Combat")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private int baseDamageToPlayer = 5;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private AudioClip swordSwingSFX;

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
    private PlayerStealth stealth;
    private Animator animator;
    private AudioSource audioSource;

    private float cooldown;
    private float lostTimer;
    private const float LOST_SIGHT_GRACE = 3f;
    public bool IsAttacking { get; set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<EnemyPathing>();
        audioSource = GetComponent<AudioSource>();

        if (TryGetComponent(out Animator anim))
            animator = anim;
        else
            Debug.LogWarning("DummyAI: No Animator component found.");

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
        bool inSight = false;

        if (distance <= detectionRange)
        {
            if (stealth.IsStealthed)
            {
                inSight = InFOV(transform, player, fovAngle, detectionRange);
            }
            else
            {
                inSight = HasLineOfSight(transform.position, player.position);
            }
        }

        switch (state)
        {
            case AIState.Patrol:
                if (animator != null) animator.SetFloat("speed", 0f);

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

                    if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }

                    lostTimer = LOST_SIGHT_GRACE;
                    RotateTowardPlayer();

                    if (animator != null)
                        animator.SetFloat("speed", agent.velocity.magnitude);

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

                    if (animator != null)
                        animator.SetFloat("speed", agent.velocity.magnitude);

                    if (lostTimer <= 0f)
                    {
                        patrol.StartPatrol();
                        state = AIState.Patrol;
                    }
                }
                break;

            case AIState.Attack:
                RotateTowardPlayer();
                if (animator != null) animator.SetFloat("speed", 0f);

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

    private bool HasLineOfSight(Vector3 origin, Vector3 targetPos)
    {
        Vector3 dir = (targetPos - origin).normalized;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, detectionRange))
        {
            return hit.transform == player;
        }
        return false;
    }

    public void PlaySwordSwingSound()
    {
        if (swordSwingSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(swordSwingSFX);
        }
    }

    public void StartAttack()
    {
        Debug.Log("Enemy attack started — attempting to deal damage.");

        if (player != null && Vector3.Distance(transform.position, player.position) <= stopDistance + 0.25f)
        {
            PlayerScript target = player.GetComponent<PlayerScript>();
            if (target != null)
            {
                target.TakeDamage(baseDamageToPlayer);
                Debug.Log($"Enemy dealt {baseDamageToPlayer} damage to player.");
            }
            else
            {
                Debug.LogWarning("PlayerScript not found on target.");
            }
        }
    }

    public void EndAttack()
    {
        Debug.Log("Enemy attack ended.");
    }

    public void DisableSwordHitbox() { }

    public void EnableSwordHitbox() { }

    public void ResetSwordHit() { }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 360-degree detection range
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 45-degree stealth detection cone
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

        // Stealth kill radius and angle
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
