using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindButton : UIInputButton
{
    protected override void Update()
    {
        base.Update();
        player.rewindInput = buttonHeldState;
        player.stoppedRewindInput = buttonReleased;
    }
}
