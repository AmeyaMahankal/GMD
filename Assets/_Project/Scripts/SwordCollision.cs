using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    [SerializeField] private int swordDamage = 10;
    [SerializeField] private Collider swordCollider;

    private bool hasHit;

    private PlayerScript playerOwner;
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
        if (playerOwner != null && playerOwner.isAttacking && !hasHit)
        {
            DummyHealth dummy = other.GetComponent<DummyHealth>() ?? other.GetComponentInParent<DummyHealth>();
            if (dummy != null)
            {
                dummy.TakeDamage(swordDamage);
                hasHit = true;
            }
        }
        else if (dummyOwner != null && !hasHit)
        {
            PlayerScript player = other.GetComponent<PlayerScript>() ?? other.GetComponentInParent<PlayerScript>();
            if (player != null)
            {
                int damage = player.isBlocking ? 2 : swordDamage;
                player.TakeDamage(damage);
                hasHit = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerOwner != null)
        {
            if (playerOwner.isAttacking && !hasHit)
            {
                DummyHealth dummy = other.GetComponent<DummyHealth>() ?? other.GetComponentInParent<DummyHealth>();
                if (dummy != null)
                {
                    dummy.TakeDamage(swordDamage);
                    hasHit = true;
                }
            }
        }
        else if (dummyOwner != null && !hasHit)
        {
            PlayerScript player = other.GetComponent<PlayerScript>() ?? other.GetComponentInParent<PlayerScript>();
            if (player != null)
            {
                int damage = player.isBlocking ? 2 : swordDamage;
                player.TakeDamage(damage);
                hasHit = true;
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
