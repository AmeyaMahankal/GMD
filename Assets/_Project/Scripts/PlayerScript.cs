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
    private Rigidbody rb;
    private Animator animator;
    private PlayerCombat playerCombat;

    private IStealth playerStealth;


    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = 9.81f;

    private bool isCrouched = false;
    private static readonly int IsCrouchedHash = Animator.StringToHash("IsCrouching");

    [SerializeField] public TextMeshProUGUI inputIndicator;
    [SerializeField] public TextMeshProUGUI  pickUpIndicator;


    [SerializeField] public GameObject  crouchedIndicator;
    [SerializeField] public GameObject  standingIndicator;
    private DroppedItem nearbyItem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerCombat = GetComponent<PlayerCombat>();
        playerStealth = GetComponent<IStealth>();
        rb = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        animator.SetBool("isGrounded", true);
        animator.SetBool("isJumping", false);
        UpdateCrouchIndicator();
    }
    
    private void FixedUpdate()
    {
        Vector3 inputVector = new Vector3(movementX, 0.0f, movementY);

        // Real movement vector
        Vector3 movement = inputVector.normalized * speed;

        // Feed actual speed into animator (you can scale this if needed for your blend tree)
        animator.SetFloat("speed", movement.magnitude);

        // Rotate player to face direction of movement
        if (inputVector.sqrMagnitude > 0.01f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector), 0.15f);
        }

        // Apply movement via Rigidbody
        Vector3 newPosition = rb.position + movement * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void Update()
    {
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

        float deadZone = 0.2f;
        movementVector.x = Mathf.Abs(movementVector.x) < deadZone ? 0 : movementVector.x;
        movementVector.y = Mathf.Abs(movementVector.y) < deadZone ? 0 : movementVector.y;

        movementX = Mathf.Sign(movementVector.x) * movementVector.x * movementVector.x;
        movementY = Mathf.Sign(movementVector.y) * movementVector.y * movementVector.y;
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
        isCrouched = !isCrouched;

        UpdateInputIndicator("B");
        animator.SetBool(IsCrouchedHash, isCrouched);
        playerStealth?.ToggleStealth();
        UpdateCrouchIndicator();
    }

    public void OnX()
    {
        UpdateInputIndicator("X");
        if (nearbyItem != null && !nearbyItem.pickedUp) {
            PickupItem(nearbyItem);
            pickUpIndicator.text = "";
            nearbyItem = null;
        }
        
        //opening a chest or a continous interctable. 
        //IsInteractingWithContinuousCollectible = !IsInteractingWithContinuousCollectible;
        //animator.SetBool(IsInteractingWithContinuousCollectibleHash, IsInteractingWithContinuousCollectible);
        Debug.Log("X");
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

    private void UpdateCrouchIndicator() {
        if(isCrouched) {
            crouchedIndicator.SetActive(true);
            standingIndicator.SetActive(false);
        } else {
            standingIndicator.SetActive(true);
            crouchedIndicator.SetActive(false);
        }
    }


    private void UpdateInputIndicator(string input) => inputIndicator.text = input;
    
    public void TakeDamage(int amount)

    {
        GetComponent<PlayerHealth>().TakeDamage(amount);
    }
    private void PickupItem(DroppedItem item)
    {
        var inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.AddItem(item.item);
    }

}
