using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Horizontal and Vertical Inputs
    public float horizontalInput;
    public float verticalInput;
    // Jump input
    [SerializeField]
    public bool jumpInput;
    // Interact input
    public bool interactInput;

    // Can player jump? Based on ground collisions
    public bool canJump = true;
    // Can player go up ladder?
    public bool canClimbLadder = false;

    public bool isClimbingLadder = false, isHangingLadder = false, isDescendingLadder = false;
    // Ground collision reference
    public GroundCollision coll { get; private set; }

    // Rewinding time input
    [SerializeField]
    public bool rewindInput;
    [HideInInspector]
    public bool stoppedRewindInput;
    [SerializeField]
    public bool timeClearInput;

    // Movement variables
    public float moveSpeed;
    public float climbSpeed;
    public float jumpVelocity;
    public float fallMultiplier;
    public float deltatimeSpeedMagnitude = 10f;

    // Rigidbody reference
    public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<GroundCollision>();
        rb.freezeRotation = true;

        // Fixing scale of movement variables with deltaTime
        moveSpeed *= deltatimeSpeedMagnitude;
        jumpVelocity *= deltatimeSpeedMagnitude;
        climbSpeed *= deltatimeSpeedMagnitude;
    }

    public void GetInput()
    {
        // Rewind dependant inputs are only set when not rewinding
        if(!TimeController.Instance.isRewindingTime)
        {
            // Getting axis inputs
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            // Getting jump input
            jumpInput = Input.GetButton("Jump");

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

    public void Move(float horizontalInput)
    {
        Jump();

        // Create move direction based on horizontal input and move speed
        Vector2 moveDirection = new Vector2(horizontalInput * moveSpeed * Time.deltaTime, rb.velocity.y);

        if(isHangingLadder)
        {
            moveDirection = new Vector2(moveDirection.x, 0f);
        }
        else if(isClimbingLadder || isDescendingLadder)
        {
            moveDirection = new Vector2(moveDirection.x, verticalInput * climbSpeed * Time.deltaTime);
        }

        // Move on direction
        rb.velocity = moveDirection;
        Debug.Log(moveDirection);

        if (isHangingLadder || isClimbingLadder || isDescendingLadder)
            return;

        // Better falling (only apply when out of ladder)
        if (rb.velocity.y <= 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }

    private void Jump()
    {
        if (canJump && jumpInput)
        {
            // Change y velocity based on jump velocity
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.velocity += Vector2.up * jumpVelocity * Time.deltaTime;

            // Immediately cancel possibility of new jump to fix bugs
            canJump = false;
        }
    }

    private void Update()
    {
        // Check grounded status every frame
        canJump = coll.isGrounded;

        GetInput();

        if(!TimeController.Instance.isRewindingTime)
            Move(horizontalInput);
    }
}
