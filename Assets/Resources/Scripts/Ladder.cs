using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            bool climbedLastUpdate = player.isClimbingLadder || player.isDescendingLadder;
            if(climbedLastUpdate)
            {
                player.isHangingLadder = true;
                player.isClimbingLadder = player.isDescendingLadder = false;
                player.rb.gravityScale = 0f;
            }

            player.canClimbLadder = true;
            if (player.verticalInput > 0)
            {
                player.isClimbingLadder = true;
                player.isDescendingLadder = player.isHangingLadder = false;
                player.rb.gravityScale = 0f;
            }
            else if (player.verticalInput < 0 && !player.coll.isGrounded)
            {
                player.isDescendingLadder = true;
                player.isClimbingLadder = player.isHangingLadder = false;
                player.rb.gravityScale = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            player.canClimbLadder = false;
            player.isHangingLadder = player.isClimbingLadder = player.isDescendingLadder = false;
            player.rb.gravityScale = 1f;
        }
    }
}
