using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using _Project.Scripts.PlayerScript.Interfaces;

public class PlayerScript : MonoBehaviour
{
    private float movementX;
    private float movementY;
    private bool isJumping = false;
    private float verticalVelocity = 0f;

    private Animator animator;
    private PlayerCombat playerCombat;

    private IStealth playerStealth;

    private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = 9.81f;


    [SerializeField] public TextMeshProUGUI inputIndicator;
    [SerializeField] public TextMeshProUGUI crouchedIndicator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerCombat = GetComponent<PlayerCombat>();
        playerStealth = GetComponent<IStealth>();
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

    public void OnMovement(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    public void OnA()
    {
        UpdateInputIndicator("A");

        if (playerStealth?.IsStealthed == true)
        {
            Debug.Log("Cannot jump while crouched.");
            return;
        }

        if (!isJumping)
        {
            isJumping = true;
            animator.SetTrigger("JumpTrigger");
            animator.SetBool("isGrounded", true);
            verticalVelocity = jumpHeight;
        }
    }

    public void OnB()
    {
        UpdateInputIndicator("B");
        playerStealth?.ToggleStealth();
        UpdateCrouchIndicator();
    }

    public void OnX()
    {
        UpdateInputIndicator("X");
    }

    public void OnY()
    {
        UpdateInputIndicator("Y");
    }

    public void OnLeftTrigger()
    {
        UpdateInputIndicator("L Trigger");
        playerCombat?.ToggleBlock();
    }

    public void OnRightTrigger()
    {
        UpdateInputIndicator("R Trigger");

        if (playerStealth?.IsStealthed == true)
        {
            playerStealth.TryStealthKill();
        }
        else
        {
            playerCombat?.PerformAttack();
        }
    }

    public void OnStart()
    {
        UpdateInputIndicator("Start");
    }

    private void UpdateCrouchIndicator()
    {
        bool isCrouched = playerStealth?.IsStealthed == true;
        crouchedIndicator.text = isCrouched ? "Crouched" : "Not Crouched.";
        crouchedIndicator.color = isCrouched ? Color.red : Color.green;
    }


    private void UpdateInputIndicator(string input) => inputIndicator.text = input;
    
    public void TakeDamage(int amount)

    {
        GetComponent<PlayerHealth>().TakeDamage(amount);
    }

}
