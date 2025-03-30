using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health = " + health);

        if (health <= 0)
        {
            // Option 1: Destroy this dummy
            Destroy(gameObject);
            
            // Or Option 2: Just log a message and do something else
            // Debug.Log(gameObject.name + " is 'dead.'");
            // this.gameObject.SetActive(false);
        }
    }
}