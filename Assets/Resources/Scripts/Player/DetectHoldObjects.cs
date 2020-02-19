using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHoldObjects : MonoBehaviour
{
    // Controller2D and Player reference
    private Controller2D playerController;
    private BoxCollider2D playerCollider;
    private PlayerControl player;
    private Animator playerAnimator;

    // Is the actor holding an object?
    public bool isHoldingObj = false;

    // Skin width to fire rays from within object
    const float skinWidth = 0.015f;

    // Distance to fire rays
    public float rayMaximumLength = 1;
    // Number of rays in axis
    private int horizontalRayCount;
    // Layers to collide with
    public LayerMask collisionMask;

    // Position to hold object
    public Vector2 holdPosition;
    public float gizmoRadius = 0.5f;

    private Vector2 rayOriginLeft, rayOriginRight;


    private void Awake()
    {
        playerController = GetComponent<Controller2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        player = GetComponent<PlayerControl>();
        playerAnimator = GetComponentInChildren<Animator>();
        horizontalRayCount = playerController.horizontalRayCount;
    }

    private void CalculateRayOrigins()
    {
        if(this.CompareTag("Player"))
        {
            rayOriginLeft = playerController.raycastOrigins.bottomLeft;
            rayOriginRight = playerController.raycastOrigins.bottomRight;
        }
        else if(this.CompareTag("TimeClone"))
        {
            Bounds bounds = playerCollider.bounds;
            bounds.Expand(-2 * skinWidth);

            rayOriginLeft = new Vector2(bounds.min.x, bounds.min.y);
            rayOriginRight = new Vector2(bounds.max.x, bounds.min.y);
        }
    }

    private void HorizontalRayCheck(Vector3 moveAmount)
    {
        // Movement direction (-1 down, 1 up)
        float directionX = Mathf.Sign(moveAmount.x);
        // Length of ray
        float rayLength = Mathf.Abs(rayMaximumLength) + skinWidth;

        for(int i = 0; i < horizontalRayCount; i++)
        {
            rayLength = Mathf.Clamp(rayLength, 0f, Mathf.Abs(rayMaximumLength) + skinWidth);
            // Ray origin on bottom left if moving down, top left if moving up
            Vector2 rayOrigin = (directionX == -1) ? rayOriginLeft : rayOriginRight;
            // Ray origin updated each iteration
            rayOrigin += Vector2.up * (playerController.horizontalRaySpacing * i);

            // Cast the given ray
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            // If it hits something, detect if it is a hold object
            if(hit)
            {
                // Other rays don't continue checking after hit first object
                rayLength = hit.distance;

                // If object is hit and player interacts, pick up
                if (hit.transform.CompareTag("HoldObject"))
                {
                    if(this.CompareTag("Player") && player.interactInput)
                    {
                        Debug.Log("Pick up");
                        // Pick up: disable collider, update object position to holdPosition and set object as child of player.
                        HoldObject holdObject = hit.transform.GetComponent<HoldObject>();
                        if (holdObject.canBePickedUp)
                        {
                            // Disable collider when carrying only if player is. If it's a clone, keep collider enabled.
                            StartCoroutine(holdObject.SendPickupSignal(true));

                            // Change rigidbody type, position and new parent
                            hit.transform.GetComponent<Rigidbody2D>().isKinematic = true;
                            hit.transform.position = (Vector2)transform.position + holdPosition;
                            hit.transform.SetParent(transform);

                            // Change isHolding to true
                            isHoldingObj = true;
                        }
                    }
                    else if(this.CompareTag("TimeClone"))
                    {
                        Clone clone = this.GetComponent<Clone>();
                        if(clone.posIndex < clone.clonePositions.Count && clone.clonePositions[clone.posIndex].input.interactInput)
                        {
                            // Pick up: disable collider, update object position to holdPosition and set object as child of player.
                            HoldObject holdObject = hit.transform.GetComponent<HoldObject>();
                            if (holdObject.canBePickedUp)
                            {
                                Debug.Log("Clone Pick up");
                                // Disable collider when carrying only if player is. If it's a clone, keep collider enabled.
                                StartCoroutine(holdObject.SendPickupSignal(false));

                                // Change rigidbody position and new parent
                                hit.transform.position = (Vector2)transform.position + holdPosition;
                                Debug.Log("Setting parent in pickup");
                                hit.transform.SetParent(transform);

                                // Change isHolding to true
                                isHoldingObj = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        CalculateRayOrigins();

        if(this.CompareTag("Player"))
            HorizontalRayCheck(player.moveAmount);
        else if(this.CompareTag("TimeClone"))
        {
            Clone clone = this.GetComponent<Clone>();
            float cloneMoveX = (clone.cloneXDirection == Clone.CloneXDirection.left) ? -1 : 1;
            HorizontalRayCheck(new Vector3(cloneMoveX, 0f, 0f));
        }

        // Checking hold state for animator
        playerAnimator.SetBool("isHolding", isHoldingObj);

        // Checking if the player can exit level based on holding object or not
        if(isHoldingObj)
        {
            // If holding, add confinerwall layer to player collision mask
            playerController.collisionMask |= (1 << LayerMask.NameToLayer("ConfinerWall"));
        }
        else
        {
            // If not, remove confinerwall layer to player collision mask
            playerController.collisionMask &= ~(1 << LayerMask.NameToLayer("ConfinerWall"));
        }
        // Note: maybe not ideal to do in update. But it works
    }

    private void OnDrawGizmos()
    {
        Vector2 holdPositionUpdated = (Vector2)transform.position + holdPosition;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(holdPositionUpdated, gizmoRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector2(holdPositionUpdated.x, holdPositionUpdated.y - gizmoRadius), new Vector2(holdPositionUpdated.x, holdPositionUpdated.y + gizmoRadius));
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector2(holdPositionUpdated.x - gizmoRadius, holdPositionUpdated.y), new Vector2(holdPositionUpdated.x + gizmoRadius, holdPositionUpdated.y));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(holdPositionUpdated, gizmoRadius/2);
    }
}
