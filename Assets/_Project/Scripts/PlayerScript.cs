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
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        animator.SetFloat("speed", movement.magnitude);

        if (movement.sqrMagnitude > 0.01f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }

        Vector3 newPosition = rb.position + movement.normalized * speed * Time.fixedDeltaTime;
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
