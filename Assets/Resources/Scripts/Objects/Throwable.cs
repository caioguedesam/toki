using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Throwable : MonoBehaviour
{
    // Throwable object variables
    private float gravity;
    private float throwYVelocity;
    public float throwHeight = 1f;
    public float timeToThrowPeak = .4f;
    public float throwSpeed;
    public float throwResistance;

    // State variables
    public bool isBeingHeld = false;
    public bool wasJustPickedUp = false;

    // References/others
    private GameObject holderObj;
    private ThrowAnchor throwAnchor;
    private Controller2D controller;
    private float moveAmountXSmoothing;

    [HideInInspector]
    public Vector3 moveAmount;

    private void Awake()
    {
        controller = GetComponent<Controller2D>();
        CalculatePhysicsVariables();
        isBeingHeld = false;
    }

    /// <summary>
    /// Calculates throwable object physics variables based on REAL WORLD PHYSICS!!
    /// </summary>
    private void CalculatePhysicsVariables()
    {
        gravity = (-2 * throwHeight) / Mathf.Pow(timeToThrowPeak, 2);
        throwYVelocity = Mathf.Abs(gravity) * timeToThrowPeak;
    }

    /// <summary>
    /// Picks up given throwable object
    /// </summary>
    /// <param name="holder">Holder to pick up object</param>
    public void PickUp(GameObject holder)
    {
        Debug.Log("called pickup");
        // Get pick up anchor
        ThrowAnchor anchor = holder.GetComponent<ThrowAnchor>();
        if(anchor != null)
        {
            holderObj = holder;
            throwAnchor = holderObj.GetComponent<ThrowAnchor>();
            // Set holder as parent
            transform.parent = holderObj.transform;
            // Set new position to holding anchor
            transform.localPosition = anchor.currentOffset;
            // Reset all throwable object collisions
            controller.collisions.Reset();
        }

        // Resetting was just picked up (set in trigger)
        wasJustPickedUp = false;
    }

    /// <summary>
    /// Updates throwable position every update
    /// </summary>
    private void UpdateThrowablePosition()
    {
        if(holderObj != null && isBeingHeld)
        {
            ThrowAnchor anchor = holderObj.GetComponent<ThrowAnchor>();
            transform.localPosition = anchor.currentOffset;
            //Debug.Log("positionupdated");
        }
    }

    /// <summary>
    /// Throws throwable object
    /// </summary>
    public void Throw()
    {
        Debug.Log("Called throw");
        // Resets parent to null
        transform.parent = null;
        // Throws in given movement direction of holder's controller2D (NEEDS REDO FOR CLONES because no controller2D)
        //float sign = Mathf.Sign(holderObj.GetComponent<Controller2D>().moveAmountPast.x);

        // Throws in given anchor hold direction
        float sign = throwAnchor.anchorDirection == ThrowAnchor.AnchorDirection.left ? -1 : 1;
        
        throwAnchor = null;
        holderObj = null;
        moveAmount.x = throwSpeed * sign;
        moveAmount.y = throwYVelocity;
    }

    /// <summary>
    /// Setting throw air resistance (X slowdown)
    /// </summary>
    private void ThrowAirResistance()
    {
        if(Mathf.Abs(moveAmount.x) > 0 && controller.collisions.below)
        {
            moveAmount.x = Mathf.SmoothDamp(moveAmount.x, 0, ref moveAmountXSmoothing, throwResistance);
        }
    }

    private void Update()
    {
        UpdateThrowablePosition();
        ThrowAirResistance();

        // Resetting y movement if collided vertically (grounded)
        if(!isBeingHeld)
        {
            if (controller.collisions.above || controller.collisions.below)
            {
                moveAmount.y = 0f;
            }
        }

        // If player is not rewinding time, move
        if (!TimeController.Instance.isRewindingTime && !isBeingHeld)
        {
            // Move player the amount gravity would move each frame
            moveAmount.y += gravity * Time.deltaTime;

            controller.Move(moveAmount * Time.deltaTime);
        }
    }
}
