using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;

using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Remaining Health = {currentHealth}");

        if (IsDead)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // Add death animation, respawn logic, etc.
    }
}


