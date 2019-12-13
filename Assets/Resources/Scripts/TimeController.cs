﻿using System.Collections;
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
    // Time current rewind started
    public float rewindStartTime = 0f;
    // Time current rewind ended
    public float rewindEndTime = 0f;
    // Total rewind time of last rewind
    public float lastRewindTime = 0f;
    // Rewind time slow/speed effect scale
    public AnimationCurve rewindTimeScaleCurve;

    // Coroutine controllers
    private bool isCheckingRewindTime = false;
    private bool isChangingTimeScale = false;

    // Original deltaTime
    private float originalFixedDeltaTime;

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
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void CheckRewind()
    {
        rewindingTime = player.rewindInput;
        if (rewindingTime)
        {
            StartCoroutine(GetRewindTime());
        }

        ChangeRewindTimeScale();

        stoppedRewind = player.stoppedRewindInput;
    }

    private IEnumerator GetRewindTime()
    {
        if(!isCheckingRewindTime)
        {
            // Stopping other coroutine calls from doing this
            isCheckingRewindTime = true;

            // Recording rewind start time
            rewindStartTime = Time.time;

            // Waiting for rewind to end
            while (rewindingTime || player.GetComponent<RewindPlayer>().isFrozen)
            {
                yield return null;
            }

            // Recording rewind end time and total time
            rewindEndTime = Time.time;
            lastRewindTime = Mathf.Min(rewindEndTime - rewindStartTime, rewindSeconds);

            Debug.Log("last rewind time: " + lastRewindTime);

            // Allowing for next rewind to be recorded
            isCheckingRewindTime = false;
        }
    }

    public void ChangeRewindTimeScale()
    {
        if(rewindingTime)
        {
            Time.timeScale = rewindTimeScaleCurve.Evaluate(Time.time - rewindStartTime);
        }
        else
        {
            Time.timeScale = 1f;
        }
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

    public bool CheckSetClones()
    {
        for(int i = 0; i < cloneList.Count; i++)
        {
            if (cloneList[i].GetComponent<Clone>().clonePositions.Count == 0)
                return false;
        }
        return true;
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