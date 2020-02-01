using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SaveData saveData;

    private void Awake()
    {
        saveData = SaveSystem.LoadGame();
        if(saveData == null)
        {
            saveData.highestLevelVisited = saveData.highestRoomVisited = 0;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On Scene loaded: " + scene.name);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("CityScene");
    }

    public void ClearData()
    {
        SaveSystem.ClearData();
    }

    private void OnDisable()
    {
        Debug.Log("disabled main menu");
    }
}
