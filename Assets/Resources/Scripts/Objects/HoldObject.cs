using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObject : MonoBehaviour
{
    // Player reference
    private Controller2D playerController;
    private PlayerControl player;
    private BoxCollider2D playerCollider;

    // Original level reference
    private GameObject levelParentObj;
    // Original transform position (for reset purposes)
    private Vector2 originalPosition;

    // Collider and Rigidbody2D reference
    private BoxCollider2D coll;
    private Rigidbody2D rigidBody;

    // Was the object just picked up?
    [SerializeField] private bool wasJustPickedUp = false;
    // Pickup cooldown
    public float pickupCooldown = 1f;
    // Throw cooldown
    public float throwCooldown = 1f;
    // Throw Force
    public float throwForce = 200f;
    // Throw Angle
    [Range(0f, 90f)] public float throwAngle = 30f;

    // Can the object be picked up?
    public bool canBePickedUp = true;
    // Can the object be thrown?
    [SerializeField] private bool canThrow = false;
    // Has clone just dropped the object?
    [SerializeField] private bool cloneDropped = false;

    // List of object positions for rewind
    [SerializeField] private List<HoldObjectPosition> positions;
    private Vector2 lastVelocity;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        player = playerObj.GetComponent<PlayerControl>();
        playerCollider = playerObj.GetComponent<BoxCollider2D>();
        playerController = playerObj.GetComponent<Controller2D>();

        levelParentObj = transform.parent.gameObject;
        originalPosition = transform.position;

        coll = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();

        positions = new List<HoldObjectPosition>();
        lastVelocity = new Vector2(0f, 0f);

        canBePickedUp = true;
    }

    public IEnumerator SendPickupSignal(bool isPlayer)
    {
        if(!wasJustPickedUp)
        {
            wasJustPickedUp = true;
            canBePickedUp = false;

            // Only change collider size if player picked up. If it was clone, don't do it.
            if(isPlayer)
            {
                // Extend player collider to consider box height also, changing size and offset accordingly.
                playerCollider.size = new Vector2(playerCollider.size.x, playerCollider.size.y + coll.size.y);
                playerCollider.offset = new Vector2(playerCollider.offset.x, playerCollider.offset.y + coll.size.y / 2f);
                // Recalculating rays based on new collider size.
                playerController.CalculateRaySpacing();
                coll.enabled = false;
            }

            // FIX THIS: DISABLE ONLY WHEN PLAYER
            
            rigidBody.isKinematic = true;

            yield return new WaitForSeconds(pickupCooldown);
            wasJustPickedUp = false;
        }
    }

    private void CheckCanThrow()
    {
        canThrow = (transform.parent.CompareTag("Player") || transform.parent.CompareTag("TimeClone")) && !wasJustPickedUp;
    }

    private void CheckForThrow()
    {
        if(transform.parent.CompareTag("Player") && canThrow && player.interactInput)
        {
            StartCoroutine(Throw(true));
        }
        else if(transform.parent.CompareTag("TimeClone"))
        {
            Clone clone = GetComponentInParent<Clone>();
            if(clone.posIndex < clone.clonePositions.Count && clone.clonePositions[clone.posIndex].input.interactInput && canThrow)
            {
                StartCoroutine(Throw(false));
            }
        }
    }

    private IEnumerator Throw(bool isPlayer)
    {
        if(!TimeController.Instance.isRewindingTime)
        {
            // Calculating throw direction and angle in radians.
            float directionX = player.facingRight ? 1 : -1;
            float angleRad = Mathf.Deg2Rad * throwAngle;

            if(isPlayer)
            {
                // Resetting player collider size to original values before holding, calculating ray spacing accordingly.
                playerCollider.size = new Vector2(playerCollider.size.x, playerCollider.size.y - coll.size.y);
                playerCollider.offset = new Vector2(playerCollider.offset.x, playerCollider.offset.y - coll.size.y / 2f);
                playerController.CalculateRaySpacing();
            }
            
            // Reset parent, then throw object with desired force and angle
            transform.SetParent(levelParentObj.transform, true);
            rigidBody.isKinematic = false;
            rigidBody.AddForce(new Vector2(directionX * throwForce * Mathf.Cos(angleRad), throwForce * Mathf.Sin(angleRad)));

            // Resetting hold boolean on actor
            transform.parent.GetComponent<DetectHoldObjects>().isHoldingObj = false;

            // Re-enable collider
            coll.enabled = true;

            yield return new WaitForSeconds(throwCooldown);
            canBePickedUp = true;
        }
    }

    private IEnumerator CloneDropped()
    {
        if(!cloneDropped)
        {
            canBePickedUp = false;
            cloneDropped = true;
            yield return new WaitForSeconds(throwCooldown);
            canBePickedUp = true;
        }
    }

    public void ResetObj()
    {
        // If player was holding object, reduce collider size accordingly
        if (transform.parent.CompareTag("Player") || transform.parent.CompareTag("TimeClone"))
        {
            bool isBeingHeld = transform.parent.GetComponent<DetectHoldObjects>().isHoldingObj;
            if(transform.parent.CompareTag("Player") && isBeingHeld)
            {
                playerCollider.size = new Vector2(playerCollider.size.x, playerCollider.size.y - coll.size.y);
                playerCollider.offset = new Vector2(playerCollider.offset.x, playerCollider.offset.y - coll.size.y / 2f);
                playerController.CalculateRaySpacing();
            }

            transform.parent.GetComponent<DetectHoldObjects>().isHoldingObj = false;
        }

        // Reset parent and position
        transform.SetParent(levelParentObj.transform, true);
        transform.position = originalPosition;
        // Reset position array
        positions.Clear();
        // Resetting pick up variables
        canBePickedUp = true;
        canThrow = false;

        // Resetting hold object properties
        coll.enabled = true;
        rigidBody.isKinematic = false;
    }

    private void ObjectRewind()
    {
        // If there are positions left on the position list, move player past one position 
        if (positions.Count > 0)
        {
            HoldObjectPosition currentPos = positions[positions.Count - 1];
            // Setting object values as stored position
            transform.position = currentPos.position;
            rigidBody.velocity = currentPos.velocity;
            // Handling new parent objects
            if(currentPos.parent != transform.parent)
            {
                if(currentPos.parent.CompareTag("Player"))
                {
                    StartCoroutine(SendPickupSignal(true));
                    transform.SetParent(currentPos.parent);
                }
                else if (currentPos.parent.CompareTag("TimeClone") && !cloneDropped)
                {
                    StartCoroutine(SendPickupSignal(false));
                    transform.SetParent(currentPos.parent);
                    /*Clone clone = currentPos.parent.GetComponent<Clone>();
                    if (clone.posIndex < clone.clonePositions.Count && clone.clonePositions[clone.posIndex].input.interactInput)
                    {
                        transform.SetParent(currentPos.parent);
                    }*/
                }
                else if(transform.parent.CompareTag("Player"))
                {
                    // Resetting player collider size to original values before holding, calculating ray spacing accordingly.
                    playerCollider.size = new Vector2(playerCollider.size.x, playerCollider.size.y - coll.size.y);
                    playerCollider.offset = new Vector2(playerCollider.offset.x, playerCollider.offset.y - coll.size.y / 2f);
                    playerController.CalculateRaySpacing();

                    // Re-enable collider and reset dynamic rb
                    coll.enabled = true;
                    rigidBody.isKinematic = false;
                    canBePickedUp = true;
                    transform.SetParent(currentPos.parent);
                }
            }
            // Additional logic for when clone is rewinding
            else if(transform.parent.CompareTag("TimeClone"))
            {
                Clone clone = GetComponentInParent<Clone>();
                if(clone.posIndex < clone.clonePositions.Count && clone.clonePositions[clone.posIndex].input.interactInput)
                {
                    // Re-enable collider and reset dynamic rb
                    coll.enabled = true;
                    rigidBody.isKinematic = false;
                    StartCoroutine(CloneDropped());
                    Debug.Log("Resetting parent");
                    transform.SetParent(levelParentObj.transform, true);
                }
            }
            //transform.SetParent(currentPos.parent);

            lastVelocity = currentPos.velocity;

            // Remove the position from the stored list
            positions.RemoveAt(positions.Count - 1);
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidBody.AddForce(new Vector2(0f, -0.001f));
        }
    }

    private void FixedUpdate()
    {
        if(!TimeController.Instance.isRewindingTime)
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //If time just stopped rewinding, apply last velocity registered to object (fixes freeze after rewind)
            if (TimeController.Instance.stoppedRewind)
                rigidBody.velocity = lastVelocity;

            cloneDropped = false;

            // Add position to register
            TimeController.Instance.AddPosition(this, positions);

            // Check for throwing
            CheckCanThrow();
            CheckForThrow();
        }
        else
        {
            ObjectRewind();
        }
    }
}

[System.Serializable]
public class HoldObjectPosition
{
    public Vector3 position;
    public Vector2 velocity;
    public Transform parent;
    public float time;

    public HoldObjectPosition(Vector3 position, Vector2 velocity, Transform parent, float time)
    {
        this.position = position;
        this.velocity = velocity;
        this.parent = parent;
        this.time = time;
    }
}
