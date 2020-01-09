using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractButton : UIInputButton
{

    protected override void Update()
    {
        pastHeldState = currentHeldState;
        currentHeldState = buttonHeldState;

        if (currentHeldState && !pastHeldState)
            player.interactInput = true;
        else
        {
            player.interactInput = false;
        }
    }
}
