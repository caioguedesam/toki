using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    // Singleton instance
    public static TimeController Instance { get; private set; }

    // Is time rewinding?
    public bool rewindingTime = false;
    // How much time to rewind
    public int rewindSeconds = 2;
    // Has rewind just stopped?
    public bool stoppedRewind = false;
    public int stoppedRewindTimeFrame = 1;
    // Player reference
    public Player player;
    // List of player time clones present in scene
    public List<GameObject> cloneList;
    // Maximum number of player time clones
    public int maxCloneLimit = 1;
    // Clone spawn cooldown
    public int cloneRespawnSeconds = 1;

    private void Awake()
    {
        // If there's nothing in the instance property, assign TimeController information to object.
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If there is, then there's another TimeController object. Destroy this one.
        else
        {
            Destroy(gameObject);
        }

        cloneList = new List<GameObject>();
    }

    private void CheckRewind()
    {
        rewindingTime = player.rewindInput;
        stoppedRewind = player.stoppedRewindInput;
    }

    public void AddPlayerPosition(GameObject gameObj, List<PlayerTimePosition> posArray)
    {
        // Making new position to store in list
        PlayerTimePosition pos = new PlayerTimePosition();
        pos.position = gameObj.transform.position;
        pos.time = Time.time;

        posArray.Add(pos);

        // Continuously removing all positions outside time threshold
        posArray.RemoveAll(x => x.time <= (Time.time - rewindSeconds));
    }

    public void AddClone(GameObject clone)
    {
        cloneList.Add(clone);
    }

    public void ActivateAllClones()
    {
        for(int i = 0; i < cloneList.Count; i++)
        {
            GameObject clone = cloneList[i];
            if (!clone.activeSelf)
                clone.SetActive(true);
        }
    }

    public void DestroyAllClones()
    {
        for (int i = 0; i < cloneList.Count; i++)
        {
            Destroy(cloneList[i]);
            cloneList.RemoveAt(i);
        }
    }

    public void CheckDestroyClones()
    {
        if (player.destroyClonesInput && cloneList.Count > 0)
            DestroyAllClones();
    }

    private void Update()
    {
        CheckRewind();
        CheckDestroyClones();
    }
}

[System.Serializable]
public class PlayerTimePosition
{
    public Vector3 position;
    public float time;
}
