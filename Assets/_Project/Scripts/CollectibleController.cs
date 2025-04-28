using System;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] public int score;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
