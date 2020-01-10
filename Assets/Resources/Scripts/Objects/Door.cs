using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool initialState;

    public void SetInitialState()
    {
        initialState = gameObject.activeSelf;
    }

    public void ActivateDoor()
    {
        Debug.Log("Toggling door!");
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void ResetObj()
    {
        Debug.Log("Resetting " + name + " to " + initialState);
        gameObject.SetActive(initialState);
    }
}
