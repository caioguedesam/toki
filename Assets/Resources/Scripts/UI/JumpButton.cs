using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpButton : UIInputButton
{
    protected override void Update()
    {
        base.Update();
        player.jumpInput = buttonHeldState;
        player.releasedJumpInput = buttonReleased;
    }
}
