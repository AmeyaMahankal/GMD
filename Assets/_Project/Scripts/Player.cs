using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 movement;
    private float movementX;
    private float movementY;
    private Animator animator;
    private Rigidbody rb;


    
    [SerializeField] private float speed = 5;
    [SerializeField] public TextMeshProUGUI  inputIndicator;
    [SerializeField] public TextMeshProUGUI  pickUpIndicator;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    
    [SerializeField] public GameObject  crouchedIndicator;
    [SerializeField] public GameObject  standingIndicator;

    private bool isCrouched = false;
    private bool IsInteractingWithContinuousCollectible = false;

    private static readonly int IsCrouchedHash = Animator.StringToHash("IsCrouching");
    public static readonly float animeSpeedHash = Animator.StringToHash("speed");
    public static readonly int IsInteractingWithContinuousCollectibleHash = Animator.StringToHash("IsInteractingWithContinuousCollectible");
    private DroppedItem nearbyItem;

    private void Awake() {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody>();

    }
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        float currentSpeed = isCrouched ? speed * crouchSpeedMultiplier : speed;

        if (movement.sqrMagnitude > 0.01f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }

        Vector3 newPosition = rb.position + movement.normalized * currentSpeed * Time.fixedDeltaTime;
        animator.SetFloat("speed", currentSpeed);
        rb.MovePosition(newPosition);
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
        Debug.Log("A");
    }

    public void OnB()
    {
        isCrouched = !isCrouched;

        animator.SetBool(IsCrouchedHash, isCrouched);
        UpdateInputIndicator("B");
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



    private void UpdateCrouchIndicator() {
        if(isCrouched) {
            crouchedIndicator.SetActive(true);
            standingIndicator.SetActive(false);
        } else {
            standingIndicator.SetActive(true);
            crouchedIndicator.SetActive(false);
        }
    }

    private void UpdateInputIndicator(string input)
    {
        inputIndicator.text = input;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DroppedItem"))
        {
            var item = other.GetComponent<DroppedItem>();
            if (item != null && !item.pickedUp)
            {
                nearbyItem = item;
                pickUpIndicator.text = "Press X To Pick Up";
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DroppedItem"))
        {
            var item = other.GetComponent<DroppedItem>();
            if (item != null && nearbyItem == item)
            {
                nearbyItem = null;
                pickUpIndicator.text = "";
            }
        }
    }

    private void PickupItem(DroppedItem item)
    {
        var inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.PickupDroppedItem(item);
    }
}
