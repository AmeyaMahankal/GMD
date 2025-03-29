using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Existing fields
    private float movementX;
    private float movementY;
    private bool isCrouched = false;
    private Animator animator;

    [SerializeField] private float speed = 5;
    [SerializeField] public TextMeshProUGUI inputIndicator;
    [SerializeField] public TextMeshProUGUI crouchedIndicator;
    
    // Jump fields
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 5f;   // Adjust as needed
    [SerializeField] private float gravity = 9.81f;  // Adjust as needed
    private float verticalVelocity = 0f;
    private bool isJumping = false;

    private void Awake() {
        animator = GetComponent<Animator>(); 
    }

    private void Start()
    {
        // Start grounded
        animator.SetBool("isGrounded", true);
        animator.SetBool("isJumping", false);
    }

    private void Update()
    {
        // --- Existing Movement ---
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        animator.SetFloat("speed", movement.magnitude);

        // Rotate only if there's input
        if (movement.sqrMagnitude > 0.01f) {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.LookRotation(movement), 
                0.15f
            );
        }

        // Translate horizontally
        transform.Translate(movement * Time.deltaTime * speed, Space.World);

        // --- Jump & Gravity ---
        if (isJumping)
        {
            // Apply gravity
            verticalVelocity -= gravity * Time.deltaTime;

            // Move vertically
            transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime, Space.World);

            // Check ground contact (assuming y=0 is the ground)
            if (transform.position.y <= 0f)
            {
                // Snap to ground
                Vector3 pos = transform.position;
                pos.y = 0f;
                transform.position = pos;

                // Reset jump state
                verticalVelocity = 0f;
                isJumping = false;

                // Update animator
                animator.SetBool("isJumping", false);
                animator.SetBool("isGrounded", true);
            }
        }
    }

    // --- Input Callback for Movement ---
    public void OnMovement(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // --- Jump Button: A ---
    public void OnA()
    {
        UpdateInputIndicator("A");
        Debug.Log("A");

        // Only jump if not currently in the air
        if (!isJumping)
        {
            isJumping = true;
            animator.SetTrigger("JumpTrigger");
            animator.SetBool("isGrounded", true);

            // Simple "impulse" jump
            // Or: verticalVelocity = Mathf.Sqrt(2f * jumpHeight * gravity);
            verticalVelocity = jumpHeight;        }
    }

    // --- Other Buttons (unchanged) ---
    public void OnB()
    {
        UpdateInputIndicator("B");
        Debug.Log("B");
    }

    public void OnX()
    {
        isCrouched = !isCrouched;
        UpdateInputIndicator("X");
        UpdateCrouchIndicator();
        Debug.Log("X");
    }

    public void OnY()
    {
        UpdateInputIndicator("Y");
        Debug.Log("Y");
    }

    public void OnLeftTrigger()
    {
        UpdateInputIndicator("L Trigger");
        Debug.Log("L Trigger");
    }

    public void OnRightTrigger()
    {
        UpdateInputIndicator("R Trigger");
        Debug.Log("R Trigger");
    }

    public void OnStart()
    {
        UpdateInputIndicator("Start");
        Debug.Log("Start");
    }

    // --- Helper Methods ---
    private void UpdateCrouchIndicator() {
        if(isCrouched) {
            crouchedIndicator.text = "Crouched";
            crouchedIndicator.color = Color.red;
        } else {
            crouchedIndicator.text = "Not Crouched.";
            crouchedIndicator.color = Color.green;
        }
    }

    private void UpdateInputIndicator(string input)
    {
        inputIndicator.text = input;
    }
}
