using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PressurePlate : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material activeMaterial;

    // Overriding inherited obsolete renderer.
    private new Renderer renderer;
    private BoxCollider2D coll;

    // Skin width to fire rays from within pressure plate
    const float skinWidth = 0.015f;
    // Number of rays to fire for checking upwards collisions
    public int verticalRayCount = 4;
    private float verticalRaySpacing;
    private RaycastOrigins raycastOrigins;
    // Length to detect plate collisions
    public float plateRayLength = 1f;
    // Layer mask to detect collisions with
    public LayerMask collisionMask;

    // Is pressure plate active?
    [SerializeField]
    public bool isActive { get; private set; }

    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
        coll = GetComponent<BoxCollider2D>();
        isActive = false;
    }

    private void Start()
    {
        CalculateRaycastOrigins();
        CalculateRaySpacing();
    }

    /// <summary>
    /// Calculates collision raycast origins based on current collider bounds and skin width.
    /// </summary>
    private void CalculateRaycastOrigins()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(-2 * skinWidth);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    /// <summary>
    /// Calculates collision raycast spacing based on ray count and collider size.
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(-2 * skinWidth);

        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// Detects collisions on specified layers to see if plate is or isn't active.
    /// </summary>
    private void DetectPlateCollisions()
    {
        for (int i = 0; i < verticalRayCount; i++)
        {
            // Ray origin on top left (since only checking collisions upwards)
            Vector2 rayOrigin = raycastOrigins.topLeft;
            // Ray origin updated each iteration
            rayOrigin += Vector2.right * (verticalRaySpacing * i);

            // Cast the given ray
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, plateRayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * plateRayLength, Color.magenta);

            // If ray hits something, it means plate should be activated
            if (hit)
            {
                Debug.Log("Coll");
                isActive = true;
                break;
            }
            // If it doesn't hit, nothing is colliding with plate
            else
            {
                Debug.Log("NoColl");
                isActive = false;
            }
        }
    }

    /// <summary>
    /// Changes plate visuals according to active state. Should change with project growth.
    /// </summary>
    /// <param name="active">Plate's active state.</param>
    private void ChangeActiveVisual(bool active)
    {
        if(active && renderer.material != activeMaterial)
        {
            renderer.material = activeMaterial;
        }
        else if(!active && renderer.material != inactiveMaterial)
        {
            renderer.material = inactiveMaterial;
        }
    }

    private void Update()
    {
        DetectPlateCollisions();
        ChangeActiveVisual(isActive);
    }
}
