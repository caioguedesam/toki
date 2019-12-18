using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerControl : MonoBehaviour
{
    // Input variables
    private float horizontalInput, verticalInput;
    public bool jumpInput;

    // Player environment variables
    public float moveSpeed = 5f;
    public float jumpHeight = 4f;
    public float timeToJumpPeak = 0.4f;
    private float jumpVelocity;
    private float playerGravity;
    public float accelerationTimeAirborne = .15f;
    public float accelerationTimeGrounded = .1f;

    Vector3 moveAmount;
    private float moveAmountXSmoothing;

    // Player references
    Controller2D controller;

    private void Awake()
    {
        controller = GetComponent<Controller2D>();
        CalculateJumpVariables();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetButtonDown("Jump");
    }

    private void Jump()
    {
        // Allow jump if jump input is pressed and player is grounded
        if(jumpInput && controller.collisions.below)
        {
            moveAmount.y = jumpVelocity;
        }
    }

    /// <summary>
    /// Calculates jump/gravity movement variables based on REAL WORLD PHYSICS!
    /// </summary>
    private void CalculateJumpVariables()
    {
        playerGravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(playerGravity) * timeToJumpPeak;
    }

    private void SmoothXMoveAmount()
    {
        float targetMoveAmountX = horizontalInput * moveSpeed;
        moveAmount.x = Mathf.SmoothDamp(moveAmount.x, targetMoveAmountX, ref moveAmountXSmoothing,
            (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
    }

    private void Update()
    {
        GetInput();

        // Resetting y movement if collided vertically (grounded)
        if(controller.collisions.above || controller.collisions.below)
        {
            moveAmount.y = 0f;
        }

        Jump();

        //moveAmount.x = horizontalInput * moveSpeed;
        SmoothXMoveAmount();
        moveAmount.y += playerGravity * Time.deltaTime;
        controller.Move(moveAmount * Time.deltaTime);
    }
}
