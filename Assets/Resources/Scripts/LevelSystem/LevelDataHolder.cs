using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataHolder : MonoBehaviour
{
    // Singleton Instance
    public static LevelDataHolder Instance { get; private set; }
    // Player ref
    public PlayerControl player;

    // Level data (modify later when there's more than one level grid maybe?)
    public GameObject levelGrid;
    private List<RoomData> roomList;

    // Player level data
    public int currentLevelIndex;
    public int currentRoomIndex;
    public Vector2 currentLevelSpawnPos;

    private void Awake()
    {
        // If there's nothing in the instance property, assign LevelDataHolder information to object.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If there is, then there's another LevelDataHolder object. Destroy this one.
        else
        {
            Destroy(gameObject);
        }

        roomList = new List<RoomData>();
        for(int i = 0; i < levelGrid.transform.childCount; i++)
        {
            RoomData room = levelGrid.transform.GetChild(i).GetComponent<RoomData>();
            room.roomIndex = i;
            roomList.Add(room);
            Debug.Log("Setting " + room.name + " with index " + room.roomIndex);
        }
    }

    private void UpdateSpawnPosition()
    {
        currentLevelSpawnPos = roomList[currentRoomIndex].spawn.transform.position;
    }

    public void RespawnPlayer()
    {
        player.transform.position = currentLevelSpawnPos;
        roomList[currentRoomIndex].ResetRoom();
    }

    private void Update()
    {
        UpdateSpawnPosition();
    }
}
