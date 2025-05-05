using UnityEngine;

public class StealthKill : MonoBehaviour
{
    public float killDistance = 2f;
    public float killAngle = 45f;
    public KeyCode killKey = KeyCode.E;

    void Update()
    {
        if (Input.GetKeyDown(killKey))
        {
            TryStealthKill();
        }
    }

    void TryStealthKill()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // Find the closest enemy within range
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= killDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        if (closestEnemy == null) return; // No enemy in range

        // Check if the player is behind the enemy
        Vector3 toPlayer = (transform.position - closestEnemy.transform.position).normalized;
        float dotProduct = Vector3.Dot(closestEnemy.transform.forward, toPlayer);
        float requiredDot = Mathf.Cos(killAngle * Mathf.Deg2Rad);

        if (dotProduct < -requiredDot)  // Ensures the player is behind enemy
        {
            PerformStealthKill(closestEnemy);
        }
    }

    void PerformStealthKill(GameObject enemy)
    {
        Debug.Log("Stealth Kill Executed!");
        Animator enemyAnimator = enemy.GetComponentInChildren<Animator>();
        if (enemyAnimator)
        {
            enemyAnimator.SetTrigger("Die");

        }

        Destroy(enemy, 4f);

    }


}
