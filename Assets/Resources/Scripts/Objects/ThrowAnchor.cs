using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAnchor : MonoBehaviour
{
    public Vector2 holdOffset;
    public Vector2 holdPosition { get; private set; }
    public float gizmoRadius = 1f;

    private void Awake()
    {
        CalculateHoldPosition();
    }

    private void CalculateHoldPosition()
    {
        holdPosition = (Vector2)transform.position + holdOffset;
    }

    private void Update()
    {
        CalculateHoldPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + holdOffset, gizmoRadius);
    }
}
