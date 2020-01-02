using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomCameraConfiner : MonoBehaviour
{
    public GameObject vcam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Changing cam");
            vcam.SetActive(true);
            // Change shake camera component in TimeController instance
            TimeController.Instance.GetComponent<CameraRewindShake>().cameraListener = vcam.GetComponent<CinemachineImpulseListener>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            vcam.SetActive(false);
        }
    }
}
