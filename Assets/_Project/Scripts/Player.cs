using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 movement;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isCrouched = false;



    [SerializeField] private float speed = 5;
    [SerializeField] public TextMeshProUGUI  inputIndicator;
    [SerializeField] public TextMeshProUGUI  crouchedIndicator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        if (movement.sqrMagnitude > 0.01f) {
         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
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
