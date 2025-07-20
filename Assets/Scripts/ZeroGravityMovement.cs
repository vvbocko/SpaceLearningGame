using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float drag = 10f;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection;

    Rigidbody rigidBody;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void PlayerMovement()
    {
        moveDirection = (orientation.forward * verticalInput) + (orientation.right * horizontalInput);

        rigidBody.AddForce (moveDirection.normalized * moveSpeed, ForceMode.Force);

    }

}
