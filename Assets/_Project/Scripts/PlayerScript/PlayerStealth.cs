using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;

public class PlayerStealth : MonoBehaviour, IStealth
{
    [SerializeField] private float killDistance = 2f;
    [SerializeField] private float killAngle = 45f;
    [SerializeField] private Animator animator;
    private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");

    private bool isCrouched = false;
    public bool IsStealthed => isCrouched;

    public void ToggleStealth()
    {
        isCrouched = !isCrouched;
        animator.SetBool(IsCrouchingHash, isCrouched);
        Debug.Log(IsStealthed ? "Player entered STEALTH mode!" : "Player exited STEALTH mode!");
    }

    public void TryStealthKill()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= killDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        if (closestEnemy == null) return;

        Vector3 toPlayer = (transform.position - closestEnemy.transform.position).normalized;
        float dotProduct = Vector3.Dot(closestEnemy.transform.forward, toPlayer);
        float requiredDot = Mathf.Cos(killAngle * Mathf.Deg2Rad);

        if (dotProduct > requiredDot)
        {
            Debug.Log("STEALTH KILL TRIGGERED");

            Animator enemyAnimator = closestEnemy.GetComponentInChildren<Animator>();
            if (enemyAnimator)
                enemyAnimator.SetTrigger("Die");

            DummyHealth health = closestEnemy.GetComponent<DummyHealth>();
            if (health != null)
            {
                health.TakeDamage(999);
            }
            else
            {
                Destroy(closestEnemy);
            }
        }
        else
        {
            Debug.Log("Stealth kill failed — player not behind the enemy.");
        }
    }
}
