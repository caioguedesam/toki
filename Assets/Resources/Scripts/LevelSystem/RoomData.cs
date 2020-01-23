using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public int roomIndex;
    public RespawnPoint spawn;

    // Room data
    private DoorButton[] doorButtons;
    private Door[] doors;

    private void Awake()
    {
        spawn = GetComponentInChildren<RespawnPoint>();
        doorButtons = GetComponentsInChildren<DoorButton>(true);
        doors = GetComponentsInChildren<Door>(true);
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
        

        // Destroying time clones
        TimeController.Instance.DestroyAllClones();
    }
}
