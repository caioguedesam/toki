using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private int thisLevel;

    private void Awake()
    {
        thisLevel = GetComponentInParent<LevelData>().levelIndex;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered");
        // if player enters end level zone, skip level
        if(collision.CompareTag("Player"))
        {
            GoToNextLevel();
        }
    }

    /// <summary>
    /// Method to call whenever player ends the current level. To be upgraded with more effects, for now just load the next scene.
    /// </summary>
    private void GoToNextLevel()
    {
        // Loads next level
        SceneManager.LoadScene(thisLevel + 1);
    }
}
