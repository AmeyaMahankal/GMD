using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    [SerializeField] private int swordDamage = 10; // can be changed in Inspector

    private void OnTriggerEnter(Collider other)
    {
        Player player = GetComponentInParent<Player>();
        if (player != null && player.isAttacking)
        {
            DummyHealth dummy = other.GetComponent<DummyHealth>();
            if (dummy != null)
            {
                // Use the Inspector-adjustable damage
                dummy.TakeDamage(swordDamage);
            }
        }
    }
}