using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(LevelDataHolder levelData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "saveFile.bin");
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            SaveData data = new SaveData(levelData);
            formatter.Serialize(stream, data);
            Debug.Log("Saved data with level " + data.highestLevelVisited + " and room " + data.highestRoomVisited);
        }
        catch
        {
            Debug.Log("Error on saving file");
        }
        finally
        {
            stream.Close();
        }
    }

    public static void ClearData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "saveFile.bin");
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            SaveData data = new SaveData();
            formatter.Serialize(stream, data);
            Debug.Log("Cleared game data");
        }
        catch
        {
            Debug.Log("Error on clearing file");
        }
        finally
        {
            stream.Close();
        }
    }

    public static SaveData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "saveFile.bin");
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            try
            {
                SaveData data = formatter.Deserialize(stream) as SaveData;
                stream.Close();

                Debug.Log("Loaded data with level " + data.highestLevelVisited + " and room " + data.highestRoomVisited);
                return data;
            }
            catch
            {
                Debug.Log("Error on loading file");
                stream.Close();
                return null;
            }
        }
        else
        {
            Debug.Log("Save File not found on path " + path);
            return null;
        }
    }


}
