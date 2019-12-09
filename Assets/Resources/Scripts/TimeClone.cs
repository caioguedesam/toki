using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeClone : MonoBehaviour
{
    // Player reference and variables
    public GameObject player;
    public PlayerController playerController;
    public TimeObject playerTimeObject;

    // Clone variables
    private TimeObject cloneTimeObject;
    // This clone's positions to execute
    public List<TimePosition> clonePositions;
    // This clone's record of executed positions
    public List<TimePosition> cloneRecord;
    // Placeholder particle for when clone is destroyed
    public ParticleSystem destroyCloneParticle;
    // Clone lifetime after ending movement
    public float cloneLifetime;

    private void Awake()
    {
        // Setting references
        player = GameObject.FindWithTag("Player");

        playerController = player.GetComponent<PlayerController>();
        playerTimeObject = player.GetComponent<TimeObject>();

        cloneTimeObject = GetComponent<TimeObject>();
        cloneTimeObject.rewindSeconds = playerTimeObject.rewindSeconds;
        clonePositions = new List<TimePosition>(playerTimeObject.lastPositions);
        cloneRecord = new List<TimePosition>();
        Debug.Log("Clone positions: " + clonePositions.Count);
    }

    /// <summary>
    /// Moves time clone by one position of the clone position list
    /// </summary>
    private void MoveClonePosition()
    {
        transform.position = clonePositions[0].position;

        clonePositions.RemoveAt(0);
    }

    /// <summary>
    /// Moves time clone when rewinding time
    /// </summary>
    private void MoveClonePosition(bool isRewinding)
    {
        if(isRewinding && cloneTimeObject.lastPositions.Count > 0)
        {
            List<TimePosition> cloneLastPositions = cloneTimeObject.lastPositions;
            clonePositions.Insert(0, cloneLastPositions[cloneLastPositions.Count - 1]);
            cloneTimeObject.lastPositions.RemoveAt(cloneLastPositions.Count - 1);
        }
    }

    /// <summary>
    /// Coroutine to destroy time clone
    /// </summary>
    private IEnumerator DestroyTimeClone()
    {
        yield return new WaitForSeconds(cloneLifetime);

        playerController.playerTimeClones.Remove(gameObject);
        Instantiate(destroyCloneParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {

        // If there's still a position to move, move
        if(clonePositions.Count > 0 && !cloneTimeObject.isRewinding)
        {
            
            MoveClonePosition();
            Debug.Log("Clone positions: " + clonePositions.Count);
        }
        else if(cloneTimeObject.isRewinding)
        {
            MoveClonePosition(true);
            Debug.Log("Clone positions: " + clonePositions.Count);
        }
        // Otherwise, destroy the clone
        else
        {
            //StartCoroutine(DestroyTimeClone());
        }
    }
}
