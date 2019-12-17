using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollision : MonoBehaviour
{
    // Layers to collide with
    public LayerMask groundLayer;
    // Is the player grounded?
    public bool isGrounded;

    // Ground collision radius
    public float collisionRadius = 0.25f;
    // Ground collision offset
    public Vector2 bottomOffset;

    private void Update()
    {
        // Create overlap circle to find ground collisions
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        // List with gizmo positions
        var positions = new Vector2[] { bottomOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
    }
}
