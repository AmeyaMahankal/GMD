using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Use Rigidbody for 3D movement
    }

    void FixedUpdate() // Use FixedUpdate for physics movement
    {
        float moveX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxisRaw("Vertical") * moveSpeed;

        rb.linearVelocity = new Vector3(moveX, rb.linearVelocity.y, moveZ); // Maintain current Y velocity (gravity)
    }
}

