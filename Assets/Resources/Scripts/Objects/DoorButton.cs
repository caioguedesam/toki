using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    // Player object reference
    private PlayerControl player;
    // List of objects to activate with switch
    public List<GameObject> listOfDoors;
    // Is the button timed?
    public bool isTimed = false;
    // Seconds for button timer
    public float buttonTimer = 3f;

    // Delay time for next toggle on button
    public float buttonToggleDelayTime = 0.3f;
    // Is switch being activated?
    private bool isButtonActivating = false;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Player pressed button
        if (collision.CompareTag("Player") && player.interactInput)
            StartCoroutine(ToggleButton());
        // Clone pressed button
        else if(collision.CompareTag("TimeClone"))
        {
            Clone clone = collision.GetComponent<Clone>();
            if(clone.clonePositions[clone.posIndex].input.interactInput)
            {
                StartCoroutine(ToggleButton());
            }
        }
    }

    private void ActivateAllButtonObjects()
    {
        for(int i = 0; i < listOfDoors.Count; i++)
        {
            Door door = listOfDoors[i].GetComponent<Door>();
            door.ActivateDoor();
        }
    }

    private IEnumerator ToggleButton()
    {
        if(!isButtonActivating)
        {
            Debug.Log("Pressed Button!");
            isButtonActivating = true;

            ActivateAllButtonObjects();
            // If the button is timed, wait timer then re-toggle all objects
            if(isTimed)
            {
                yield return new WaitForSeconds(buttonTimer);
                ActivateAllButtonObjects();
            }
            else
                yield return new WaitForSeconds(buttonToggleDelayTime);

            isButtonActivating = false;
        }
    }
}
