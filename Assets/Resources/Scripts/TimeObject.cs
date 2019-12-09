using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Caio Guedes de Azevedo Mota - 12/2019 */

/// <summary>
/// TimeObject: Script to be attached to any class that can be rewinded when player presses button.
/// This includes the player and other objects that may require this.
/// 
/// May redo this to be handled on only one Game Object, if needed.
/// </summary>
public class TimeObject : MonoBehaviour
{
    // Object positions
    private List<TimePosition> positions;
    // Last positions
    public List<TimePosition> lastPositions;
    // Seconds to get last positions and rewind time
    public float rewindSeconds;
    // Is object reversing or not?
    public bool isRewinding = false;
    // Has object just went out of rewind?
    public bool stoppedRewind = false;
    // Is object frozen in time or not?
    public bool isFrozen = false;

    // Time when current rewind began
    private float rewindStartTime;

    /// <summary>
    /// Call every update to rewind position by one position in stored list.
    /// </summary>
    public void RewindTime()
    {
        //Debug.Log("Rewinding");
        // Setting new object position
        transform.position = positions[positions.Count - 1].position;
        // Storing said position in lastPositions
        lastPositions.Insert(0, positions[positions.Count - 1]);
        // Removing said position to use previous position in next call
        positions.RemoveAt(positions.Count - 1);
    }

    /// <summary>
    /// Clears current list of stored positions.
    /// </summary>
    public void ClearPositionList()
    {
        positions.Clear();
    }

    /// <summary>
    /// Clears current list of rewinded positions.
    /// </summary>
    public void ClearLastPositionList()
    {
        lastPositions.Clear();
    }

    private void Start()
    {
        rewindStartTime = 0;
        // Setting references
        positions = new List<TimePosition>();
        lastPositions = new List<TimePosition>();
    }

    private void Update()
    {
        // If certain button is pressed, begin rewind
        isRewinding = Input.GetButton("Fire1");
        // If certain button is released, rewind just ended
        stoppedRewind = Input.GetButtonUp("Fire1");
    }

    private void FixedUpdate()
    {
        // Rewing done in FixedUpdate because of framerate independence

        // If time is not rewinding, keep saving current positions
        if (!isRewinding)
        {
            isFrozen = false;
            rewindStartTime = 0;

            // Making new position to store in list
            TimePosition pos = new TimePosition();
            pos.position = transform.position;
            pos.time = Time.time;

            positions.Add(pos);

            // Continuously removing all positions outside time threshold
            positions.RemoveAll(x => x.time <= (Time.time - rewindSeconds));
            //Debug.Log(positions.Count);
        }
        // Otherwise, keep changing transform position to last positions in position list, removing one at a time.
        else if(positions.Count > 0)
        {
            // Set time rewind began
            if (rewindStartTime == 0)
                rewindStartTime = Time.time;
            // Rewind Time!
            RewindTime();

            //Debug.Log(positions.Count);
        }
        // If all stored positions end, object is frozen in time
        else
        {
            isFrozen = true;
        }
    }
}

/// <summary>
/// Time position to store position and time registered.
/// </summary>
public class TimePosition
{
    public Vector3 position;
    public float time;
}
