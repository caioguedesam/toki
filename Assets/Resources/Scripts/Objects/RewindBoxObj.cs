using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindBoxObj : MonoBehaviour
{
    // Object position list for rewind
    public List<BoxColliderTimePosition> positions;

    // Animator reference for enabling/disabling
    private Animator animator;

    private void Awake()
    {
        positions = new List<BoxColliderTimePosition>();
        animator = GetComponent<Animator>();
    }

    private void RewindObject()
    {
        // If there are positions left on the position list, move player past one position 
        /*if (positions.Count > 0)
        {
            BoxColliderTimePosition currentPos = positions[positions.Count - 1];

            // Setting sprite
            GetComponent<SpriteRenderer>().sprite = currentPos.sprite;

            // Setting new collider size
            GetComponent<BoxCollider2D>().size = currentPos.colliderSize;

            // Remove the position from the stored list
            positions.RemoveAt(positions.Count - 1);
        }*/
        animator.SetBool("isRewinding", true);
    }

    /// <summary>
    /// Checks to see if the animator for the player should be active relative to time rewind.
    /// </summary>
    private void ActivateAnimator()
    {
        animator.enabled = !TimeController.Instance.isRewindingTime;
    }

    private void FixedUpdate()
    {
        if(!TimeController.Instance.isRewindingTime)
        {
            TimeController.Instance.AddPosition(gameObject, positions);
            animator.SetBool("isRewinding", false);
        }
        else
        {
            RewindObject();
        }
    }
}
