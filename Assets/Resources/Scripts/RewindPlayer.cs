using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindPlayer : MonoBehaviour
{
    public List<PlayerTimePosition> playerPositions;
    public List<PlayerTimePosition> playerRecord;
    public bool isFrozen = false;
    public bool canSpawnNewClone = true;
    private bool stoppedRewind;

    public GameObject timeClonePrefab;
    private Rigidbody2D rb;

    private void Start()
    {
        playerPositions = new List<PlayerTimePosition>();
        playerRecord = new List<PlayerTimePosition>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void PlayerRewind()
    {
        if(playerPositions.Count > 0)
        {
            PlayerTimePosition currentPos = playerPositions[playerPositions.Count - 1];
            transform.position = currentPos.position;
            playerRecord.Insert(0, currentPos);
            playerPositions.RemoveAt(playerPositions.Count - 1);
        }
        else
        {
            isFrozen = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void SpawnClone()
    {
        if(TimeController.Instance.cloneList.Count < TimeController.Instance.maxCloneLimit)
        {
            Debug.Log("Spawn Clone!");
            GameObject clone;
            clone = Instantiate(timeClonePrefab, transform.position, Quaternion.identity);
            clone.SetActive(false);

            TimeController.Instance.AddClone(clone);
            StartCoroutine(WaitToSpawnClone());

            playerRecord.Clear();
            Debug.Log("Player Record Cleared!");
        }
    }

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

        if (TimeController.Instance.rewindingTime)
        {
            PlayerRewind();
            if(isFrozen && canSpawnNewClone)
            {
                SpawnClone();
            }
                
        }
        else
        {
            if (stoppedRewind && canSpawnNewClone)
            {
                SpawnClone();
            }
            isFrozen = false;
            rb.constraints = RigidbodyConstraints2D.None;

            TimeController.Instance.ActivateAllClones();

            TimeController.Instance.AddPlayerPosition(gameObject, playerPositions);

            // Not sure if I really need this. Address later on.
            playerRecord.Clear();
        }
    }
}
