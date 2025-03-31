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

    // ATTACK
    [Header("Attack Settings")]
    [Tooltip("If true, we are currently in an attack animation.")]
    public bool isAttacking = false;

    // ----------------------------------
    // NEW BLOCKING FIELD
    // ----------------------------------
    [Header("Block Settings")]
    [Tooltip("If true, player is in a 'blocking' stance and will take reduced damage.")]
    public bool isBlocking = false;

    // Player Health
    [Header("Player Health")]
    [SerializeField] private int playerHealth = 100;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.SetBool("isGrounded", true);
        animator.SetBool("isJumping", false);
    }

    private void Update()
    {
        Vector3 movement = new Vector3(movementX, 0f, movementY);
        animator.SetFloat("speed", movement.magnitude);

        if (movement.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movement),
                0.15f
            );
        }

        transform.Translate(movement * Time.deltaTime * speed, Space.World);

        // Jump & Gravity
        if (isJumping)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime, Space.World);

            if (transform.position.y <= 0f)
            {
                Vector3 pos = transform.position;
                pos.y = 0f;
                transform.position = pos;

                verticalVelocity = 0f;
                isJumping = false;
                animator.SetBool("isJumping", false);
                animator.SetBool("isGrounded", true);
            }
        }
    }

    // --- Movement Input ---
    public void OnMovement(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // --- Jump Button A ---
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

    // Other Buttons
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

    // --- Left Trigger: Now used for Blocking ---
    public void OnLeftTrigger()
    {
        UpdateInputIndicator("L Trigger");
        Debug.Log("L Trigger");

        // Toggle blocking
        isBlocking = !isBlocking;

        // Later, you could trigger a block animation here:
        // animator.SetBool("isBlocking", isBlocking);

        if (isBlocking)
        {
            Debug.Log("Player started blocking!");
        }
        else
        {
            Debug.Log("Player stopped blocking!");
        }
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

    private void PerformAttack()
    {
        animator.SetTrigger("attack");
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Attack ended - no longer dealing damage on collision.");
    }

    // Player Takes Damage
    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Remaining Health = " + playerHealth);

        if (playerHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }
}
