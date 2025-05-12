using UnityEngine;

public class DummyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private int baseDamageToPlayer = 5;
    [SerializeField] private float detectionRange = 15f;

    [Header("Vision Cone Settings")]
    [SerializeField] private float fovAngle = 90f; // 180° full cone = 90° to each side

    [Header("Stealth Kill Gizmo Settings")]
    [SerializeField] private float killDistance = 2f;
    [SerializeField] private float killAngle = 45f;

    private float attackCooldown = 5f;
    private float currentCooldown;

    private PlayerScript player;
    private bool playerDetected = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerScript>();
        currentCooldown = attackCooldown;
    }

    private void Update()
    {
        if (player == null || player.IsStealthed) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Detect player only if within range AND inside FOV
        if (distanceToPlayer <= detectionRange && InFOV(transform, player.transform, fovAngle, detectionRange))
        {
            playerDetected = true;
        }
        else
        {
            playerDetected = false;
            return;
        }

        if (distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0f)
            {
                int damage = player.isBlocking ? 2 : baseDamageToPlayer;
                player.TakeDamage(damage);
                Debug.Log($"Dummy attacked Player for {damage} damage. Next attack in 5 seconds.");
                currentCooldown = attackCooldown;
            }
        }
    }

    private bool InFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(checkingObject.position, maxRadius, overlaps);

        for (int i = 0; i < count; i++)
        {
            if (overlaps[i] != null && overlaps[i].transform == target)
            {
                Vector3 directionBetween = (target.position - checkingObject.position).normalized;
                directionBetween.y = 0;

                float angle = Vector3.Angle(checkingObject.forward, directionBetween);

                if (angle <= maxAngle)
                {
                    Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, maxRadius))
                    {
                        if (hit.transform == target)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPos);

        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    private void StopMoving() { }

    private void OnDrawGizmos()
    {
        // Draw vision cone in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -fovAngle, 0) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fovAngle, 0) * transform.forward * detectionRange;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // Stealth kill cone
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killDistance);

        Quaternion leftKill = Quaternion.Euler(0, killAngle + 90, 0);
        Quaternion rightKill = Quaternion.Euler(0, -killAngle - 90, 0);

        Vector3 leftDir = leftKill * -transform.forward * killDistance;
        Vector3 rightDir = rightKill * -transform.forward * killDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir);
        Gizmos.DrawLine(transform.position, transform.position + rightDir);
    }
}
