using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    private float movementX;
    private float movementY;
    private bool isCrouched = false;
    private Animator animator;

    [SerializeField] private float speed = 5;
    [SerializeField] public TextMeshProUGUI inputIndicator;
    [SerializeField] public TextMeshProUGUI crouchedIndicator;
    
    // Jump fields
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 5f;   
    [SerializeField] private float gravity = 9.81f;  
    private float verticalVelocity = 0f;
    private bool isJumping = false;

    // ------------------------------------
    // ATTACK FIELDS (no OverlapSphere now)
    // ------------------------------------
    [Header("Attack Settings")]
    [Tooltip("If true, means we are currently in an attack animation and can deal damage on sword collision.")]
    public bool isAttacking = false;

    private void Awake() 
    {
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
        Vector3 movement = new Vector3(movementX, 0f, movementY);
        animator.SetFloat("speed", movement.magnitude);

        // Rotate only if there's input
        if (movement.sqrMagnitude > 0.01f) 
        {
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
            verticalVelocity -= gravity * Time.deltaTime;
            transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime, Space.World);

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

        if (!isJumping)
        {
            isJumping = true;
            animator.SetTrigger("JumpTrigger");
            animator.SetBool("isGrounded", true);

            verticalVelocity = jumpHeight;        
        }
    }

    // --- Other Buttons ---
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

    // --- Left Trigger: now does nothing (or remove if desired) ---
    public void OnLeftTrigger()
    {
        UpdateInputIndicator("L Trigger");
        Debug.Log("L Trigger");
        // No attack code here now
    }

    // --- Right Trigger: Attack ---
    public void OnRightTrigger()
    {
        UpdateInputIndicator("R Trigger");
        Debug.Log("R Trigger");

        PerformAttack();
    }

    public void OnStart()
    {
        UpdateInputIndicator("Start");
        Debug.Log("Start");
    }

    // --- Helper Methods ---
    private void UpdateCrouchIndicator()
    {
        if (isCrouched)
        {
            crouchedIndicator.text = "Crouched";
            crouchedIndicator.color = Color.red;
        }
        else
        {
            crouchedIndicator.text = "Not Crouched.";
            crouchedIndicator.color = Color.green;
        }
    }

    private void UpdateInputIndicator(string input)
    {
        inputIndicator.text = input;
    }

    // ---------------------------------
    // ATTACK HELPER (NO OverlapSphere)
    // ---------------------------------
    private void PerformAttack()
    {
        // Trigger the attack animation
        animator.SetTrigger("attack");

        // Mark that we are currently in an attack
        isAttacking = true;
    }

    // Called via Animation Event near the end of the clip
    public void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Attack ended - no longer dealing damage on collision.");
    }
}
