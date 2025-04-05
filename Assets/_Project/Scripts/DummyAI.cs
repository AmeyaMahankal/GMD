using UnityEngine;

public class DummyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 1.5f; // Stop moving when this close
    [SerializeField] private int baseDamageToPlayer = 5; // Normal damage (2 if blocking)

    // We'll do damage every 5 seconds once in range
    private float attackCooldown = 5f; // 5-second countdown
    private float currentCooldown;

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        // Initialize so we can deal damage once 5 seconds have passed
        currentCooldown = attackCooldown;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        float detectionRange = 15f;

        if (distanceToPlayer > detectionRange)
        {
            // Player is out of detection range — do nothing
            return;
        }

        // Player is within detection range — proceed as usual

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

    private void StopMoving()
    {
        // Intentionally blank: no movement
        // (Optional) you could add an idle animation or something here
    }
}
