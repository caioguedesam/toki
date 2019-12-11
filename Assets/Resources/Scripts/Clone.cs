using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    private GameObject player;
    public RewindPlayer playerRewind;
    public List<PlayerTimePosition> clonePositions;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        playerRewind = player.GetComponent<RewindPlayer>();
        clonePositions = new List<PlayerTimePosition>(playerRewind.playerRecord);
        Debug.Log("clonepos: " + clonePositions.Count);
    }
}
