using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    private GameObject player;
    public RewindPlayer playerRewind;
    public List<PlayerTimePosition> clonePositions;

    public int posIndex = 0;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");

        playerRewind = player.GetComponent<RewindPlayer>();
        clonePositions = new List<PlayerTimePosition>(playerRewind.playerRecord);
        Debug.Log("playerrecord: " + playerRewind.playerRecord.Count);
        Debug.Log("clonepos: " + clonePositions.Count);
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
