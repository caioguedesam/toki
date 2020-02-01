using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int highestLevelVisited;
    public int highestRoomVisited;

    public SaveData()
    {
        highestLevelVisited = highestRoomVisited = 0;
    }

    public SaveData(LevelDataHolder levelData)
    {
        highestLevelVisited = levelData.highestLevelVisited;
        highestRoomVisited = levelData.highestRoomVisited;
    }
}
