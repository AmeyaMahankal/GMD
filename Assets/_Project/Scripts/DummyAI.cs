using UnityEngine;

public class DummyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private int baseDamageToPlayer = 5;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float fovAngle = 60f;

    [Header("Stealth Kill Gizmo Settings")]
    [SerializeField] private float killDistance = 2f;
    [SerializeField] private float killAngle = 45f;

    private float attackCooldown = 5f;
    private float currentCooldown;

    private Player player;
    private bool isInFOV = false;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        currentCooldown = attackCooldown;
    }

    private void Update()
    {
        if (player == null || player.IsStealthed) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        isInFOV = InFOV(transform, player.transform, fovAngle, detectionRange);

        if (!isInFOV || distanceToPlayer > detectionRange)
            return;

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

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    private void StopMoving() { }

    private bool InFOV(Transform checkingObject, Transform target, float maxAngle, float maxRadius)
    {
        Vector3 directionBetween = (target.position - checkingObject.position).normalized;
        directionBetween.y = 0;

        float angle = Vector3.Angle(checkingObject.forward, directionBetween);

        if (angle > maxAngle)
            return false;

        Ray ray = new Ray(checkingObject.position, target.position - checkingObject.position);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRadius))
        {
            return hit.transform == target;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(fovAngle, transform.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fovAngle, transform.up) * transform.forward * detectionRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (player != null)
        {
            Gizmos.color = isInFOV ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, (player.transform.position - transform.position).normalized * detectionRange);
        }

        // Stealth kill gizmos (cone behind enemy)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killDistance);

        Quaternion leftRotation = Quaternion.Euler(0, killAngle + 90, 0);
        Quaternion rightRotation = Quaternion.Euler(0, -killAngle - 90, 0);

        Vector3 leftDir = leftRotation * -transform.forward * killDistance;
        Vector3 rightDir = rightRotation * -transform.forward * killDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir);
        Gizmos.DrawLine(transform.position, transform.position + rightDir);
    }
}
