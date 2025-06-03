using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    [SerializeField] private int swordDamage = 10;
    [SerializeField] private Collider swordCollider;

    private bool hasHit;

    private PlayerScript playerOwner;
    private PlayerCombat combat;
    private DummyAI dummyOwner;

    private void Start()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;

        playerOwner = GetComponentInParent<PlayerScript>();
        dummyOwner = GetComponentInParent<DummyAI>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerOwner != null && combat.IsAttacking && !hasHit)
        {
            DummyHealth dummy = other.GetComponent<DummyHealth>() ?? other.GetComponentInParent<DummyHealth>();
            if (dummy != null)
            {
                dummy.TakeDamage(swordDamage);
                hasHit = true;
            }
        }
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (combat != null)
        {
            if (combat.IsAttacking && !hasHit)
            {
                DummyHealth dummy = other.GetComponent<DummyHealth>() ?? other.GetComponentInParent<DummyHealth>();
                if (dummy != null)
                {
                    dummy.TakeDamage(swordDamage);
                    hasHit = true;
                }
            }
        }
       
    }

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

    public void ResetHit()
    {
        hasHit = false;
    }
}
