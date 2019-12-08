using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeClone : MonoBehaviour
{
    // Player reference and variables
    public GameObject player;
    public PlayerController playerController;
    public TimeObject playerTimeObject;

    // This clone's positions to execute
    public List<TimePosition> clonePositions;
    // Placeholder particle for when clone is destroyed
    public ParticleSystem destroyCloneParticle;

    private void Awake()
    {
        // Setting references
        player = GameObject.FindWithTag("Player");

        playerController = player.GetComponent<PlayerController>();
        playerTimeObject = player.GetComponent<TimeObject>();
        clonePositions = new List<TimePosition>(playerTimeObject.lastPositions);
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
    /// Coroutine to destroy time clone
    /// </summary>
    private IEnumerator DestroyTimeClone()
    {
        yield return new WaitForSeconds(3);

        playerController.playerTimeClones.Remove(gameObject);
        Instantiate(destroyCloneParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Update()
    {
        // If there's still a position to move, move
        if(clonePositions.Count > 0)
        {
            Debug.Log("Clone positions: " + clonePositions.Count);
            MoveClonePosition();
        }
        // Otherwise, destroy the clone
        else
        {
            StartCoroutine(DestroyTimeClone());
        }
    }
}
