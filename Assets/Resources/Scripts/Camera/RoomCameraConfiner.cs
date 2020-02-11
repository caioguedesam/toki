using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomCameraConfiner : MonoBehaviour
{
    public GameObject vcam;
    [SerializeField] private int roomIndex;
    [SerializeField] private int levelIndex;

    private void Start()
    {
        levelIndex = GetComponentInParent<LevelData>().levelIndex;
        roomIndex = GetComponentInParent<RoomData>().roomIndex;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Changing cam");
            vcam.SetActive(true);
            // Change shake camera component in TimeController instance
            TimeController.Instance.GetComponent<CameraRewindShake>().cameraListener = vcam.GetComponent<CinemachineImpulseListener>();

            // Setting level data for new room entered
            Debug.Log("Setting new room index " + roomIndex);
            if(roomIndex != LevelDataHolder.Instance.currentRoomIndex)
            {
                // Deleting clones from past room
                TimeController.Instance.DestroyAllClones();
            }
            LevelDataHolder.Instance.currentRoomIndex = roomIndex;

            LevelDataHolder.Instance.CheckForGameProgress(levelIndex, roomIndex);
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
