using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerControl : MonoBehaviour
{
    // Input variables
    [SerializeField]
    private float horizontalInput, verticalInput;
    public bool jumpInput;
    public bool interactInput;
    public bool timeClearInput;
    public bool rewindInput, stoppedRewindInput;
    public bool releasedJumpInput = false;

    // Player environment variables
    public float moveSpeed = 5f;
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpPeak = 0.4f;
    private float maxJumpVelocity;
    private float minJumpVelocity;
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
        if (!TimeController.Instance.isRewindingTime)
        {
            // Getting axis inputs
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            // Getting jump input
            jumpInput = Input.GetButton("Jump");
            releasedJumpInput = Input.GetButtonUp("Jump");
            // Other inputs
            timeClearInput = Input.GetButtonDown("Fire2");
            interactInput = Input.GetButtonDown("Fire3");
        }

        // Getting rewind input
        rewindInput = Input.GetButton("Fire1");
        stoppedRewindInput = Input.GetButtonUp("Fire1");
    }

    public void SetInputFromPosition(TimePositionInput input)
    {
        jumpInput = input.jumpInput;
        interactInput = input.interactInput;
        timeClearInput = input.timeClearInput;
        // Rewind shouldn't be set because this is called when rewinding!
        //rewindInput = input.rewindInput;
    }

    private void Jump()
    {
        // Allow jump if jump input is pressed and player is grounded
        if(jumpInput && controller.collisions.below)
        {
            moveAmount.y = maxJumpVelocity;
        }

        // If jump button is released in midair, end jump earlier (by adding the reduced minimum jump and stopping)
        if(releasedJumpInput && moveAmount.y > minJumpVelocity)
        {
            moveAmount.y = minJumpVelocity;
        }
    }

    /// <summary>
    /// Calculates jump/gravity movement variables based on REAL WORLD PHYSICS!
    /// </summary>
    private void CalculateJumpVariables()
    {
        playerGravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        maxJumpVelocity = Mathf.Abs(playerGravity) * timeToJumpPeak;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(playerGravity) * minJumpHeight);
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
        // Calculate X movement based on smoothing function
        SmoothXMoveAmount();

        // If player is not rewinding time, move
        if(!TimeController.Instance.isRewindingTime)
        {
            // Move player the amount gravity would move each frame
            moveAmount.y += playerGravity * Time.deltaTime;

            controller.Move(moveAmount * Time.deltaTime);
        }
    }
}
