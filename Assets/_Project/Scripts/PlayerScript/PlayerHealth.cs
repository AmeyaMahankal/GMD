using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [Header("Optional")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private DeathManager deathManager;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public bool IsDead => currentHealth <= 0;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.setHealth(currentHealth, maxHealth);

    }

    // Removed automatic damage
    private void Update()
    {
        if (IsDead) return;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Remaining Health = {currentHealth}");
        healthBar.setHealth(currentHealth, maxHealth);

        if (IsDead)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        deathManager.ShowGameOver();
        // Add death animation, respawn logic, etc.
    }
}
