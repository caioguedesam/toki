using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAnchor : MonoBehaviour
{
    public Vector2 holdOffsetRight;
    public Vector2 holdOffsetLeft;
    public Vector2 currentOffset { get; private set; }
    public float gizmoRadius = 1f;

    private Controller2D holderController;

    private void Awake()
    {
        holderController = GetComponent<Controller2D>();
        CalculateHoldPosition();
    }

    private void CalculateHoldPosition()
    {
        float sign = Mathf.Sign(holderController.moveAmountPast.x);
        currentOffset = sign == -1 ? holdOffsetLeft : holdOffsetRight;
    }

    private void Update()
    {
        CalculateHoldPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + holdOffsetLeft, gizmoRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + holdOffsetRight, gizmoRadius);
    }
}
