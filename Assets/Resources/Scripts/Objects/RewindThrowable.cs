using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindThrowable : MonoBehaviour
{
    private Throwable throwableObj;
    public List<ThrowableTimePosition> objectPositions;

    private void Awake()
    {
        throwableObj = GetComponent<Throwable>();
        objectPositions = new List<ThrowableTimePosition>();
    }

    private void Rewind()
    {
        if(objectPositions.Count > 0)
        {
            ThrowableTimePosition currentPos = objectPositions[objectPositions.Count - 1];
            transform.position = currentPos.position;
            // Set object new movement state at past position
            throwableObj.moveAmount = currentPos.moveAmount;
            // Remove the position from the stored list
            objectPositions.RemoveAt(objectPositions.Count - 1);
        }
    }

    private void FixedUpdate()
    {
        if(TimeController.Instance.isRewindingTime)
        {
            Rewind();
        }
        else
        {
            TimeController.Instance.AddPosition(throwableObj, objectPositions);
        }
    }
}

public class ThrowableTimePosition
{
    public Vector3 position;
    public float time;
    public bool isBeingHeld;

    public Vector3 moveAmount;
}
