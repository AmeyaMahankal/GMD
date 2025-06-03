using _Project.Scripts.PlayerScript.Interfaces;
using UnityEngine;

using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealth
{
    [Header("Optional")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public bool IsDead => currentHealth <= 0;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    
    private float damageTimer = 0f;
    private float damageInterval = 1f; // 1 second

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.setHealth(currentHealth, maxHealth);
    }
    
    private void Update()
    {
        if (IsDead) return;

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageInterval)
        {
            TakeDamage(2);
            damageTimer = 0f;
        }
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
        // Add death animation, respawn logic, etc.
    }
}


