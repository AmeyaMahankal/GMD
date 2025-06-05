using _Project.Scripts.PlayerScript.Interfaces;
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
            Debug.Log(gameObject.name + " is dead!");
            Destroy(gameObject);
        }
    }
}
