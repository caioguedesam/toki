using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAnchor : MonoBehaviour
{
    // Anchor variables (one for left movement, one for right movement)
    public Vector2 holdOffsetRight;
    public Vector2 holdOffsetLeft;
    public Vector2 currentOffset { get; private set; }

    // Gizmo variables
    public float gizmoRadius = 1f;
    public Color gizmoColor = Color.green;

    // References
    private Controller2D holderController;

    private void Awake()
    {
        holderController = GetComponent<Controller2D>();
        CalculateHoldPosition();
    }

    /// <summary>
    /// Calculates hold position based on holder controller's past move position (NEEDS REDO FOR CLONE)
    /// </summary>
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
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + holdOffsetLeft, gizmoRadius);
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + holdOffsetRight, gizmoRadius);
    }
}
