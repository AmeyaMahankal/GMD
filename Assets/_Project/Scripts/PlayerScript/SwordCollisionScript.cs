using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;

public class SwordCollisionScript : MonoBehaviour
{
    [SerializeField] private int swordDamage = 10;
    [SerializeField] private Collider swordCollider;

    private bool hasHit;
    private PlayerCombat ownerCombatant;

    private void Start()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;

        ownerCombatant = GetComponentInParent<PlayerCombat>();
    }

    private void DealDamage(Collider other)
    {

        // If not ICombatant, try to deal damage to DummyHealth (non-combatant)
        DummyHealth dummy = other.GetComponent<DummyHealth>() ?? other.GetComponentInParent<DummyHealth>();
        if (dummy != null && !hasHit)
        {
            dummy.TakeDamage(swordDamage);
            hasHit = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ownerCombatant != null && ownerCombatant.IsAttacking)
        {
            DealDamage(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (ownerCombatant != null && ownerCombatant.IsAttacking)
        {
            DealDamage(other);
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