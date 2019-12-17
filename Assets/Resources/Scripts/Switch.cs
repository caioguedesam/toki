using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    // Player object reference
    public Player player;
    // List of objects to activate with switch
    public List<GameObject> listOfObjects;
    // Will the button be activated?
    private bool activateCondition;

    // Delay time for next toggle
    public float switchToggleDelayTime = 1f;
    // Is switch being activated?
    private bool isSwitchActivating = false;

    private void Awake()
    {
        // Setting player reference
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    /// <summary>
    /// This activates all of the objects tied to this switch.
    /// </summary>
    private void ActivateAllSwitchObjects()
    {
        for(int i = 0; i < listOfObjects.Count; i++)
        {
            GameObject obj = listOfObjects[i];
            ActivateDoorSwitchObjects(obj);
        }
    }

    /// <summary>
    /// This activates a door object tied to this switch.
    /// </summary>
    /// <param name="obj">The game object to activate in case it's a door.</param>
    private void ActivateDoorSwitchObjects(GameObject obj)
    {
        Debug.Log(obj.tag);
        if(obj.tag == "Door")
        {
            SimpleDoor door = obj.GetComponent<SimpleDoor>();
            door.ActivateDoor();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // If player presses button, start switch activation
        if(collision.tag == "Player" && player.interactInput)
        {
            StartCoroutine(ActivateSwitch());
        }
        else if(collision.tag == "TimeClone")
        {
            Clone clone = collision.GetComponent<Clone>();
            bool cloneInteract = clone.clonePositions[clone.posIndex].input.interactInput;

            if (cloneInteract)
                StartCoroutine(ActivateSwitch());
        }
    }

    /// <summary>
    /// Coroutine to call when switch is being activated. Also deals with activation delay.
    /// </summary>
    private IEnumerator ActivateSwitch()
    {
        if(!isSwitchActivating)
        {
            Debug.Log("Pressed button!");
            isSwitchActivating = true;

            ActivateAllSwitchObjects();
            yield return new WaitForSeconds(switchToggleDelayTime);

            isSwitchActivating = false;
        }
    }
}
