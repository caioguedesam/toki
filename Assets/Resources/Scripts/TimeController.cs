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
    // Player reference
    public Player player;
    // List of time clones present in scene
    //public List<GameObject> cloneList;

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

        //cloneList = new List<GameObject>();
    }

    private void CheckRewind()
    {
        rewindingTime = player.rewindInput;
        stoppedRewind = player.stoppedRewindInput;
    }

    public void AddPosition(GameObject gameObj, List<TimePosition> posArray)
    {
        // Making new position to store in list
        TimePosition pos = new TimePosition();
        pos.position = gameObj.transform.position;
        pos.time = Time.time;

        posArray.Add(pos);

        // Continuously removing all positions outside time threshold
        posArray.RemoveAll(x => x.time <= (Time.time - rewindSeconds));
    }

    /*public void AddClone(GameObject clone)
    {
        cloneList.Add(clone);
    }*/

    private void Update()
    {
        CheckRewind();
    }
}

[System.Serializable]
public class TimePosition
{
    public Vector3 position;
    public float time;
}
