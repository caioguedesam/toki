using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public int levelIndex;
    public int roomIndex;
    public RespawnPoint spawn;

    // Room data
    private DoorButton[] doorButtons;
    private Door[] doors;
    private HoldObject[] holdObjects;
    private void Awake()
    {
        spawn = GetComponentInChildren<RespawnPoint>();
        doorButtons = GetComponentsInChildren<DoorButton>(true);
        doors = GetComponentsInChildren<Door>(true);
        holdObjects = GetComponentsInChildren<HoldObject>(true);
    }

    private void Start()
    {
        /*for(int i = 0; i < doors.Length; i++)
        {
            doors[i].SetInitialState();
        }*/
    }

    public void ResetRoom()
    {
        // Reset objects
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].ResetObj();
        }
        for (int i = 0; i < doorButtons.Length; i++)
        {
            doorButtons[i].ResetObj();
        }
        for(int i = 0; i < holdObjects.Length; i++)
        {
            holdObjects[i].ResetObj();
        }

        // Destroying time clones
        TimeController.Instance.DestroyAllClones();
    }
}
