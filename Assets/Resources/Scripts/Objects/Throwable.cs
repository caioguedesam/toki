using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Throwable : MonoBehaviour
{
    private float gravity;
    private float throwYVelocity;
    public float throwHeight = 1f;
    public float timeToThrowPeak = .4f;
    public float throwSpeed;
    public float throwResistance;

    public bool isBeingHeld = false;
    public bool wasJustPickedUp = false;

    private GameObject holderObj;
    private Controller2D controller;
    private float moveAmountXSmoothing;

    Vector3 moveAmount;

    private void Awake()
    {
        controller = GetComponent<Controller2D>();
        CalculatePhysicsVariables();
        isBeingHeld = false;
    }

    private void CalculatePhysicsVariables()
    {
        gravity = (-2 * throwHeight) / Mathf.Pow(timeToThrowPeak, 2);
        throwYVelocity = Mathf.Abs(gravity) * timeToThrowPeak;
    }

    public void PickUp(GameObject holder)
    {
        Debug.Log("called pickup");
        ThrowAnchor anchor = holder.GetComponent<ThrowAnchor>();
        if(anchor != null)
        {
            holderObj = holder;
            transform.parent = holderObj.transform;
            transform.localPosition = anchor.currentOffset;
            controller.collisions.Reset();
        }

        wasJustPickedUp = false;
    }

    private void UpdateThrowablePosition()
    {
        if(holderObj != null && isBeingHeld)
        {
            ThrowAnchor anchor = holderObj.GetComponent<ThrowAnchor>();
            transform.localPosition = anchor.currentOffset;
            Debug.Log("positionupdated");
        }
    }

    public void Throw()
    {
        transform.parent = null;
        float sign = Mathf.Sign(holderObj.GetComponent<Controller2D>().moveAmountPast.x);
        holderObj = null;
        moveAmount.x = throwSpeed * sign;
        moveAmount.y = throwYVelocity;
    }

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
            Debug.Log(moveAmount.y);
            // Move player the amount gravity would move each frame
            moveAmount.y += gravity * Time.deltaTime;

            controller.Move(moveAmount * Time.deltaTime);
        }
    }
}
