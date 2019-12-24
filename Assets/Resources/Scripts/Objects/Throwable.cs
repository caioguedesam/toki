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

    public bool isBeingHeld = false;
    public bool wasJustPickedUp = false;

    private PlayerControl player;
    private Controller2D controller;
    private Rigidbody2D triggerRB;

    Vector3 moveAmount;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        controller = GetComponent<Controller2D>();
        triggerRB = GetComponentInChildren<Rigidbody2D>();
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
            transform.parent = holder.transform;
            transform.localPosition = anchor.holdOffset;
        }

        wasJustPickedUp = false;
    }

    public void Throw()
    {
        transform.parent = null;
    }

    private void Update()
    {

        // Resetting y movement if collided vertically (grounded)
        if (controller.collisions.above || controller.collisions.below)
        {
            moveAmount.y = 0f;
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
