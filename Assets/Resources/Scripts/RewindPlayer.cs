using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindPlayer : MonoBehaviour
{
    public List<TimePosition> playerPositions;
    public bool isFrozen = false;

    private Rigidbody2D rb;

    private void Start()
    {
        playerPositions = new List<TimePosition>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void PlayerRewind()
    {
        if(playerPositions.Count > 0)
        {
            transform.position = playerPositions[playerPositions.Count - 1].position;
            playerPositions.RemoveAt(playerPositions.Count - 1);
        }
        else
        {
            isFrozen = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void FixedUpdate()
    {
        if(TimeController.Instance.rewindingTime)
        {
            PlayerRewind();
        }
        else
        {
            isFrozen = false;
            rb.constraints = RigidbodyConstraints2D.None;

            TimeController.Instance.AddPosition(gameObject, playerPositions);
        }
    }
}
