using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public void ActivateDoor()
    {
        // Toggles active state of gameobject (dumb way of closing/opening doors for now)
        Debug.Log("Toggling door");
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
