using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Throwable))]
public class ThrowTrigger : MonoBehaviour
{
    // Trigger variables
    public bool isBeingHeld = false;
    public bool isBeingHeldByPlayer = false;
    private PlayerControl player;
    private Throwable throwableObj;

    private void Awake()
    {
        // Setting references
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        throwableObj = GetComponentInParent<Throwable>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // TODO: Bugfixes with objects keeping held state and not working

        // If trigger collides with player and player interacts
        if (collision.tag == "Player" && player.interactInput)
        {
            Debug.Log("Changed hold state");
            // If object is not being held and player isn't holding something else, pick up
            if (!isBeingHeld && !throwableObj.wasJustPickedUp && !player.isHoldingObject)
            {
                Debug.Log("was just picked up");
                // Setting was just picked up (reset in object)
                throwableObj.wasJustPickedUp = true;
                throwableObj.PickUp(collision.gameObject);
                player.isHoldingObject = true;
            }
            // Else, if this is beign held, throw
            else if(isBeingHeld)
            {
                Debug.Log("was thrown");
                throwableObj.Throw();
                //StartCoroutine(WaitForNewPickup(player));
                player.isHoldingObject = false;
            }
            // Change held state (BUGS probably here!!)
            isBeingHeld = !isBeingHeld;
            isBeingHeldByPlayer = isBeingHeld;
            throwableObj.isBeingHeld = isBeingHeld;
        }
        // Else, if trigger collides with clone (and object is not being held by player TODO BUG: player might be holding another object)
        else if(collision.tag == "TimeClone" && !isBeingHeldByPlayer)
        {
            Clone clone = collision.GetComponent<Clone>();
            bool cloneInteract = clone.clonePositions[clone.posIndex].input.interactInput;
            if(cloneInteract)
            {
                Debug.Log("Changed hold state");
                // If object is not being held and player isn't holding something else, pick up
                if (!isBeingHeld && !throwableObj.wasJustPickedUp && !clone.isHoldingObject)
                {
                    Debug.Log("was just picked up");
                    // Setting was just picked up (reset in object)
                    throwableObj.wasJustPickedUp = true;
                    throwableObj.PickUp(collision.gameObject);
                    clone.isHoldingObject = true;
                }
                // Else, if this is beign held, throw
                else if (isBeingHeld)
                {
                    Debug.Log("was thrown");
                    throwableObj.Throw();
                    //StartCoroutine(WaitForNewPickup(player));
                    clone.isHoldingObject = false;
                }
                // Change held state (BUGS probably here!!)
                isBeingHeld = !isBeingHeld;
                throwableObj.isBeingHeld = isBeingHeld;
            }
        }
    }

    /*private IEnumerator WaitForNewPickup(PlayerControl player)
    {
        yield return new WaitForSeconds(throwableObj.timeToThrowPeak);
        player.isHoldingObject = false;
    }*/
}
