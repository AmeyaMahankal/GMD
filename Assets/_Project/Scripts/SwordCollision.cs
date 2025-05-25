using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    [SerializeField] private int swordDamage = 10; // Can be changed in Inspector
    [SerializeField] private Collider swordCollider;

    private bool hasHit;

    private void Start()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerScript player = GetComponentInParent<PlayerScript>();
        if (player != null && player.isAttacking && !hasHit)
        {
            DummyHealth dummy = other.GetComponent<DummyHealth>();
            if (dummy != null)
            {
                dummy.TakeDamage(swordDamage);
                hasHit = true;
            }
        }
    }

    // Called via animation events
    public void EnableSwordCollider()
    {
        if (swordCollider != null)
            swordCollider.enabled = true;
    }

    public void DisableSwordCollider()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    // Called when attack animation ends
    public void ResetHit()
    {
        hasHit = false;
    }
}
