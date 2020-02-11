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

    // Player data for saving and loading (move later to more general GameDataHolder)
    public int highestLevelVisited { get; private set; }
    public int highestRoomVisited { get; private set; }

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

        levelGrid = GameObject.FindWithTag("LevelData");
        currentLevelIndex = levelGrid.GetComponent<LevelData>().levelIndex;

        roomList = new List<RoomData>();
        for(int i = 0; i < levelGrid.transform.childCount; i++)
        {
            RoomData room = levelGrid.transform.GetChild(i).GetComponent<RoomData>();
            room.levelIndex = currentLevelIndex;
            room.roomIndex = i;
            roomList.Add(room);
        }

        SaveData data = SaveSystem.LoadGame();
        if(data != null)
        {
            highestLevelVisited = data.highestLevelVisited;
            highestRoomVisited = data.highestRoomVisited;
        }
    }

    private void Start()
    {
        // Player spawns in the room registered in save data (later add logic for level select)
        player.transform.position = roomList[highestRoomVisited].spawn.transform.position;
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

    public void CheckForGameProgress(int level, int room)
    {
        Debug.Log("Checking for game progress...");
        if (level > highestLevelVisited)
        {
            highestLevelVisited = level;
            highestRoomVisited = room;
            Debug.Log("new level reached: " + highestLevelVisited);
            Debug.Log("new room reached: " + highestRoomVisited);

            SaveSystem.SaveGame(Instance);
        }
        else if(room > highestRoomVisited)
        {
            highestRoomVisited = room;
            Debug.Log("new room reached: " + highestRoomVisited);
            SaveSystem.SaveGame(Instance);
        }

        // Updating current level index in leveldataholder and level information
        // if level changes, update level grid game object accordingly
        /*if(currentLevelIndex != levelGrid.GetComponent<LevelData>().levelIndex)*/
        if(levelGrid == null)
        {
            levelGrid = GameObject.FindWithTag("LevelData");
        }
        currentLevelIndex = levelGrid.GetComponent<LevelData>().levelIndex;
    }

    private void Update()
    {
        UpdateSpawnPosition();
    }
}
