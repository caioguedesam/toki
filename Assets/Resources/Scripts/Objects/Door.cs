using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void ActivateDoor()
    {
        Debug.Log("Toggling door!");
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
