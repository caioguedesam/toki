using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTrigger : MonoBehaviour
{
    public bool isBeingHeld = false;
    private PlayerControl player;
    private Throwable throwableObj;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        throwableObj = GetComponentInParent<Throwable>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && player.interactInput)
        {
            Debug.Log("Changed hold state");
            if (!isBeingHeld && !throwableObj.wasJustPickedUp)
            {
                Debug.Log("was just picked up");
                throwableObj.wasJustPickedUp = true;
                throwableObj.PickUp(collision.gameObject);
            }
            else if(isBeingHeld)
            {
                Debug.Log("was thrown");
                throwableObj.Throw();
            }
            isBeingHeld = !isBeingHeld;
            throwableObj.isBeingHeld = isBeingHeld;
        }
    }
}
