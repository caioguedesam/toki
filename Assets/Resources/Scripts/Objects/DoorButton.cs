using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    // Player object reference
    private PlayerControl player;
    // List of objects to activate with switch
    public List<GameObject> listOfDoors;

    // Delay time for next toggle on button
    public float buttonToggleDelayTime = 1f;
    // Is switch being activated?
    private bool isButtonActivating = false;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && player.interactInput)
            StartCoroutine(ToggleButton());
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
            yield return new WaitForSeconds(buttonToggleDelayTime);

            isButtonActivating = false;
        }
    }
}
