using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SaveData saveData;
    public int sceneCount;

    private void Awake()
    {
        saveData = SaveSystem.LoadGame();
        if(saveData == null)
        {
            saveData.highestLevelVisited = saveData.highestRoomVisited = 0;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Scene Count gets - 1 because main menu is scene 0.
        sceneCount = SceneManager.sceneCount - 1;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("On Scene loaded: " + scene.name);
    }

    /// <summary>
    /// Loads game with current save data.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(saveData.highestLevelVisited);
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
