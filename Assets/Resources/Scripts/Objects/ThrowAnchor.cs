using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class ThrowAnchor : MonoBehaviour
{
    // Anchor variables (one for left movement, one for right movement)
    public Vector2 holdOffsetRight;
    public Vector2 holdOffsetLeft;
    public AnchorDirection anchorDirection;
    public Vector2 currentOffset { get; private set; }

    // Gizmo variables
    public float gizmoRadius = .25f;
    public Color gizmoColor = Color.green;

    // References
    private Controller2D holderController;

    public enum AnchorDirection
    {
        left, right, noDirection
    }

    private void Awake()
    {
        holderController = GetComponent<Controller2D>();
        CalculateHoldPosition();
        anchorDirection = AnchorDirection.noDirection;
    }

    /// <summary>
    /// Calculates hold position based on holder controller's past move position (NEEDS REDO FOR CLONE)
    /// </summary>
    private void CalculateHoldPosition()
    {
        // Different behavior for clones
        if(this.tag == "TimeClone")
        {
            Clone clone = GetComponent<Clone>();
            currentOffset = clone.cloneXDirection == Clone.CloneXDirection.left ? holdOffsetLeft : holdOffsetRight;
            anchorDirection = clone.cloneXDirection == Clone.CloneXDirection.left ? AnchorDirection.left : AnchorDirection.right;
        }
        else
        {
            float sign = Mathf.Sign(holderController.moveAmountPast.x);
            currentOffset = sign == -1 ? holdOffsetLeft : holdOffsetRight;
            anchorDirection = sign == -1 ? AnchorDirection.left : AnchorDirection.right;
        }
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
