using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone : MonoBehaviour
{
    // Player reference
    private GameObject player;
    // Player rewind script ref
    public RewindPlayer playerRewind;
    // List of positions that clone must traverse
    public List<PlayerTimePosition> clonePositions;
    public CloneXDirection cloneXDirection;

    public bool isHoldingObject = false;

    private float cloneRewindTime = 0f;
    private float cloneSpawnTime = 0f;
    // Current position in position list that clone is assigned to
    public int posIndex = 0;

    // Clone sprite renderer
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Setting references
        player = GameObject.FindWithTag("Player");
        
        playerRewind = player.GetComponent<RewindPlayer>();
        clonePositions = new List<PlayerTimePosition>(playerRewind.playerRecord);
        cloneRewindTime = TimeController.Instance.lastRewindTime;
        cloneSpawnTime = TimeController.Instance.rewindStartTime;
        cloneXDirection = CloneXDirection.noDirection;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Debug.Log("playerrecord: " + playerRewind.playerRecord.Count);
        Debug.Log("clonepos: " + clonePositions.Count);
    }

    /*private void Start()
    {
        // When clone starts, clean 
        CleanClonePositions();
    }

    private void CleanClonePositions()
    {
        /*if(cloneRewindTime != TimeController.Instance.rewindSeconds)
            clonePositions.RemoveAll(x => x.time < (cloneSpawnTime - cloneRewindTime));
    }*/

    /// <summary>
    /// Method to move clone in the next position.
    /// </summary>
    private void MoveClone()
    {
        // If position index hasn't reached end of position list, move to next position.
        if(posIndex < clonePositions.Count)
        {
            transform.position = clonePositions[posIndex].position;
            spriteRenderer.sprite = clonePositions[posIndex].sprite;
            spriteRenderer.flipX = !clonePositions[posIndex].facingRight;

            // Calculate Clone X Direction
            CalculateCloneXDirection();

            // And increment index.
            posIndex++;
        }
    }

    /// <summary>
    /// Handles clone movement when rewinding time.
    /// </summary>
    private void RewindClone()
    {
        // If position index hasn't reached start of position list, move to past position.
        if(posIndex > 0)
        {
            // By decrementing index.
            posIndex--;
            transform.position = clonePositions[posIndex].position;
            spriteRenderer.sprite = clonePositions[posIndex].sprite;
            spriteRenderer.flipX = !clonePositions[posIndex].facingRight;

            // Calculate Clone X Direction
            CalculateCloneXDirection();
        }
    }

    /// <summary>
    /// Calculates clone's X direction based on most recent movement.
    /// </summary>
    private void CalculateCloneXDirection()
    {
        if (posIndex != 0)
        {
            float sign = Mathf.Sign(clonePositions[posIndex].position.x - clonePositions[posIndex - 1].position.x);
            cloneXDirection = (sign == -1) ? CloneXDirection.left : CloneXDirection.right;
        }
    }

    private void FixedUpdate()
    {
        // Rewind is handled in FixedUpdate for framerate purposes.
        if(!TimeController.Instance.isRewindingTime)
        {
            MoveClone();
        }
        else
        {
            RewindClone();
        }
    }

    public enum CloneXDirection
    {
        left, right, noDirection
    }
}
