using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    // Horizontal and Vertical Inputs
    private float horizontalInput;
    private float verticalInput;
    // Jump input
    private bool jumpInput;

    // Can player jump? Based on ground collisions
    private bool canJump = true;
    // Ground collision reference
    private GroundCollision coll;
    // Time object reference
    private TimeObject timeObj;
    // All player time clones
    public GameObject timeClonePrefab;
    // Can player spawn new time clone?
    private bool canSpawnTimeClone = false;
    [SerializeField]
    public List<GameObject> playerTimeClones;

    // Movement variables
    public float moveSpeed;
    public float jumpVelocity;
    public float fallMultiplier;
    public float deltatimeSpeedMagnitude = 10f;

    // Rigidbody reference
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<GroundCollision>();
        timeObj = GetComponent<TimeObject>();
        playerTimeClones = new List<GameObject>();

        // Fixing scale of movement variables with deltaTime
        moveSpeed = moveSpeed * deltatimeSpeedMagnitude;
        jumpVelocity = jumpVelocity * deltatimeSpeedMagnitude;
    }

    public void GetInput()
    {
        // Getting axis inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = 0f;
        // Getting jump input
        jumpInput = Input.GetButton("Jump");
    }

    public void Move(float horizontalInput)
    {
        Jump();

        // Create move direction based on horizontal input and move speed
        Vector2 moveDirection = new Vector2(horizontalInput * moveSpeed * Time.deltaTime, rb.velocity.y);
        // Move on direction
        rb.velocity = moveDirection;

        // Better falling
        if(rb.velocity.y <= 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
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

    public void CreateTimeClone()
    {
        GameObject cloneObj;
        cloneObj = Instantiate(timeClonePrefab, transform.position, Quaternion.identity);

        /*TimeClone timeClone = cloneObj.GetComponent<TimeClone>();
        timeClone.playerController = this;
        timeClone.playerTimeObject = timeObj;
        timeClone.clonePositions = timeObj.lastPositions;
        Debug.Log("last positions: " + timeObj.lastPositions.Count);
        Debug.Log("clone method positions: " + timeClone.clonePositions.Count);*/

        cloneObj.SetActive(false);

        playerTimeClones.Add(cloneObj);
    }

    /// <summary>
    /// Activates all time clones of player when stopped rewinding.
    /// </summary>
    public void ActivateAllTimeClones()
    {
        foreach(GameObject clone in playerTimeClones)
        {
            if (!clone.activeSelf)
                clone.SetActive(true);
        }

        // Clearing last position list after activating all time clones
        timeObj.ClearLastPositionList();
    }

    private void Update()
    {
        // Check grounded status every frame
        canJump = coll.isGrounded;

        GetInput();

        // Move properly if player's not rewinding time and not frozen
        if (!timeObj.isReversing && !timeObj.isFrozen)
        {
            Move(horizontalInput);
            rb.constraints = RigidbodyConstraints2D.None;
            // After first move, can spawn a new time clone
            canSpawnTimeClone = true;
            // Activate all time clones when they aren't activated
            ActivateAllTimeClones();
        }
        // If player is frozen, stop movement entirely
        else if (timeObj.isFrozen)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            
            // Only spawn new time clone once per rewind
            if(canSpawnTimeClone)
                CreateTimeClone();
            canSpawnTimeClone = false;
        }
    }
}
