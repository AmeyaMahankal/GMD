using UnityEngine;
using UnityEngine.AI;

//
// Attach *both* this script and EnemyPathing to the same GameObject.
// EnemyPathing handles patrol; DummyAI takes over for chase / attack.
//

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyPathing))]
public class DummyAI : MonoBehaviour
{
    // ──────────── Inspector ────────────
    [Header("References")]
    public Transform player;                       // optional; auto-found at runtime

    [Header("Movement / Combat")]
    [SerializeField] private float stopDistance       = 1.5f;
    [SerializeField] private int   baseDamageToPlayer = 5;
    [SerializeField] private float attackCooldown     = 5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange     = 15f;
    [SerializeField] private float fovAngle           = 90f;   // 90° left + 90° right

    [Header("Stealth-Kill")]
    [SerializeField] private float killDistance       = 2f;
    [SerializeField] private float killAngle          = 45f;

    // ──────────── Internal state ────────────
    private enum AIState { Patrol, Chase, Attack }
    private AIState       state = AIState.Patrol;

    private NavMeshAgent  agent;
    private EnemyPathing  patrol;
    private PlayerScript  playerScript;

    private float         cooldown;
    private float         lostTimer;
    private const float   LOST_SIGHT_GRACE = 3f;     // how long to “search” before giving up

    // ───────────────────────────────────────────────────────────────
    #region Unity lifecycle
    private void Awake()
    {
        agent  = GetComponent<NavMeshAgent>();
        patrol = GetComponent<EnemyPathing>();
        CachePlayerReference();
    }

    private void Start()
    {
        cooldown = attackCooldown;
    }

    private void Update()
    {
        if (playerScript == null || playerScript.IsStealthed) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool  inSight  = distance <= detectionRange &&
                         InFOV(transform, player, fovAngle, detectionRange);

        switch (state)
        {
            // ───────────────────── PATROL ─────────────────────
            case AIState.Patrol:
                if (inSight)
                {
                    patrol.StopPatrol();
                    state = AIState.Chase;
                }
                break;

            // ───────────────────── CHASE ──────────────────────
            case AIState.Chase:
                if (inSight)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    lostTimer = LOST_SIGHT_GRACE;

                    if (distance <= stopDistance)
                        state = AIState.Attack;
                }
                else
                {
                    lostTimer -= Time.deltaTime;
                    if (lostTimer <= 0f)
                    {
                        patrol.StartPatrol();
                        state = AIState.Patrol;
                    }
                }
                break;

            // ───────────────────── ATTACK ─────────────────────
            case AIState.Attack:
                agent.isStopped = true;             // stand still while striking
                cooldown -= Time.deltaTime;

                if (cooldown <= 0f)
                {
                    int dmg = playerScript.isBlocking ? 2 : baseDamageToPlayer;
                    playerScript.TakeDamage(dmg);
                    cooldown = attackCooldown;
                }

                if (distance > stopDistance)        // player moved away
                    state = AIState.Chase;
                break;
        }
    }
    #endregion
    // ───────────────────────────────────────────────────────────────
    #region Helper methods
    /// <summary>Finds / caches PlayerScript and its Transform.</summary>
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

    /// <summary>Returns true if <paramref name="target"/> is inside a forward cone.</summary>
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
    #endregion
    // ───────────────────────────────────────────────────────────────
    #region Gizmos (scene-view debug only)
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // vision radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // FOV edges
        Vector3 left  = Quaternion.AngleAxis( fovAngle, transform.up) * transform.forward * detectionRange;
        Vector3 right = Quaternion.AngleAxis(-fovAngle, transform.up) * transform.forward * detectionRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, left);
        Gizmos.DrawRay(transform.position, right);

        // ray to player: green if in FOV, red if not
        if (player != null)
        {
            bool inside = InFOV(transform, player, fovAngle, detectionRange);
            Gizmos.color = inside ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position,
                           (player.position - transform.position).normalized * detectionRange);
        }

        // stealth-kill radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killDistance);

        // back-stab window
        Vector3 killL = Quaternion.AngleAxis( killAngle, transform.up) * -transform.forward * killDistance;
        Vector3 killR = Quaternion.AngleAxis(-killAngle, transform.up) * -transform.forward * killDistance;
        Gizmos.color = new Color(1f, 0f, 1f);   // magenta
        Gizmos.DrawRay(transform.position, killL);
        Gizmos.DrawRay(transform.position, killR);
    }
#endif
    #endregion
}
