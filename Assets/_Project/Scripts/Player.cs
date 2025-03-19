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



    private float speed = 5;
    public TextMeshPro text;

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
        //text.text = "A";
        Debug.Log("A");
    }

    public void OnB()
    {
        //text.text = "B";
        Debug.Log("B");

    }

    public void OnX()
    {
        //text.text = "X";
        Debug.Log("X");

    }

    public void OnY()
    {
        //text.text = "Y";
        Debug.Log("Y");

    }

    public void OnLeftTrigger()
    {
        //text.text = "L Trigger";
        Debug.Log("L Trigger");

    }

    public void OnRightTrigger()
    {
        //text.text = "R Trigger";
        Debug.Log("R Trigger");
    }

    public void OnStart()
    {
        //text.text = "Start";
        Debug.Log("Start");
    }
}
