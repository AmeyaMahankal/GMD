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
        // Find the nearest enemy
        GameObject enemy = GameObject.FindWithTag("Enemy");
        if (enemy == null) return;

        // Check distance
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        if (distance > killDistance) return;

        // Check if within 45-degree cone behind enemy
        Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(enemy.transform.forward, -directionToEnemy);

        float requiredDot = Mathf.Cos(killAngle * Mathf.Deg2Rad); // Convert angle to dot product range
        if (dotProduct >= requiredDot) // Ensures within 45 degrees behind enemy
        {
            PerformStealthKill(enemy);
        }
    }

    void PerformStealthKill(GameObject enemy)
    {
        Debug.Log("Stealth Kill Executed!");

        Destroy(enemy, 1f); 
    }


}
