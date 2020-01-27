using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindPlayer : MonoBehaviour
{
    private PlayerControl player;
    // Player's stored positions
    public List<PlayerTimePosition> playerPositions;
    // Player record of positions (is sent to time clones)
    public List<PlayerTimePosition> playerRecord;
    // Is the player frozen in time?
    public bool isFrozen = false;
    // Can the player spawn a new time clone?
    public bool canSpawnNewClone = true;
    // Has the player just stopped time rewind?
    private bool stoppedRewind;

    // Time clone GameObject
    public GameObject timeClonePrefab;
    // Rigidbody2D ref
    //private Rigidbody2D rb;

    private void Start()
    {
        // Setting references
        player = GetComponent<PlayerControl>();
        playerPositions = new List<PlayerTimePosition>();
        playerRecord = new List<PlayerTimePosition>();
        //rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Handles player movement while rewinding time.
    /// </summary>
    public void PlayerRewind()
    {
        // If there are positions left on the position list, move player past one position 
        if(playerPositions.Count > 0)
        {
            PlayerTimePosition currentPos = playerPositions[playerPositions.Count - 1];
            transform.position = currentPos.position;
            // Set player inputs along with position
            player.SetInputFromPosition(currentPos.input);
            // Setting sprite
            player.GetComponentInChildren<SpriteRenderer>().sprite = currentPos.sprite;
            player.facingRight = currentPos.facingRight;
            // Insert the position on the record for the clone
            playerRecord.Insert(0, currentPos);
            // Remove the position from the stored list
            playerPositions.RemoveAt(playerPositions.Count - 1);
        }
        // If there are no positions left, player is frozen in time
        else
        {
            isFrozen = true;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    /// <summary>
    /// Method to spawn a new time clone.
    /// </summary>
    private void SpawnClone()
    {
        // If there are less clones than the clone limit established in TimeController, spawn
        if(TimeController.Instance.cloneList.Count < TimeController.Instance.maxCloneLimit)
        {
            Debug.Log("Spawn Clone!");
            GameObject clone;
            clone = Instantiate(timeClonePrefab, transform.position, Quaternion.identity);
            // Initially sets clone to inactive state
            clone.SetActive(false);

            TimeController.Instance.AddClone(clone);
            // Wait to be able to spawn new clones
            StartCoroutine(WaitToSpawnClone());

            // Clear the player record
            //playerRecord.Clear();
            //Debug.Log("Player Record Cleared!");
        }
    }

    /// <summary>
    /// Waits time established in TimeController for new time clone spawn
    /// </summary>
    public IEnumerator WaitToSpawnClone()
    {
        canSpawnNewClone = false;
        // Wait for clone spawn time
        yield return new WaitForSeconds(TimeController.Instance.cloneRespawnSeconds);
        // Wait until object is not frozen to allow respawn again
        while (isFrozen)
            yield return null;
        // When object is not frozen, allow respawn IF clone limit hasn't been reached
        if(TimeController.Instance.cloneList.Count <= TimeController.Instance.maxCloneLimit)
            canSpawnNewClone = true;
    }

    private void Update()
    {
        stoppedRewind = TimeController.Instance.stoppedRewind;
    }

    private void FixedUpdate()
    {

        // If time is rewinding, call rewind and spawn clone at the end
        if (TimeController.Instance.isRewindingTime)
        {
            PlayerRewind();
            if(isFrozen && canSpawnNewClone)
            {
                SpawnClone();
                playerRecord.Clear();
            }
                
        }
        else
        {
            // If time just stopped rewinding and player can spawn a new clone, spawn
            if (stoppedRewind && canSpawnNewClone)
            {
                SpawnClone();
                playerRecord.Clear();
            }
            // If time isn't rewinding, player isn't frozen in time.
            isFrozen = false;
            //rb.constraints = RigidbodyConstraints2D.None;

            TimeController.Instance.ActivateAllClones();

            // Add new position every physics update
            TimeController.Instance.AddPosition(gameObject, playerPositions);

            // Not sure if I really need this. Address later on.
            //playerRecord.Clear();
        }
    }
}
