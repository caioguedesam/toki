using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    private GameObject player;
    public RewindPlayer playerRewind;
    public List<PlayerTimePosition> clonePositions;

    private float cloneRewindTime = 0f;
    private float cloneSpawnTime = 0f;
    public int posIndex = 0;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");

        playerRewind = player.GetComponent<RewindPlayer>();
        clonePositions = new List<PlayerTimePosition>(playerRewind.playerRecord);
        cloneRewindTime = TimeController.Instance.lastRewindTime;
        cloneSpawnTime = TimeController.Instance.rewindStartTime;

        Debug.Log("playerrecord: " + playerRewind.playerRecord.Count);
        Debug.Log("clonepos: " + clonePositions.Count);
    }

    private void Start()
    {
        CleanClonePositions();
    }

    private void CleanClonePositions()
    {
        /*if(cloneRewindTime != TimeController.Instance.rewindSeconds)
            clonePositions.RemoveAll(x => x.time < (cloneSpawnTime - cloneRewindTime));*/
    }

    private void MoveClone()
    {
        if(posIndex < clonePositions.Count)
        {
            transform.position = clonePositions[posIndex].position;
            posIndex++;
        }
    }

    private void RewindClone()
    {
        if(posIndex > 0)
        {
            posIndex--;
            transform.position = clonePositions[posIndex].position;
        }
    }

    private void Update()
    {
        if(!TimeController.Instance.rewindingTime)
        {
            MoveClone();
        }
        else
        {
            RewindClone();
        }
    }
}
