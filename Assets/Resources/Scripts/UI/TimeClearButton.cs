using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeClearButton : UIInputButton
{
    private bool canPressAgain = true;
    public float pressAgainTime = 0.01f;

    protected override void Update()
    {
        if (buttonHeldState)
            StartCoroutine(PressAgainTime());
        else
        {
            if (pastHeldState)
            {
                Debug.Log("button released");
                buttonReleased = true;
                canPressAgain = true;
            }
            else
            {
                buttonReleased = false;
            }
        }

        pastHeldState = buttonHeldState;

        if (buttonHeldState && canPressAgain)
            player.timeClearInput = true;
        else
            player.timeClearInput = false;
    }

    private IEnumerator PressAgainTime()
    {
        if (canPressAgain)
        {
            yield return new WaitForSeconds(pressAgainTime);
            canPressAgain = false;
        }
    }
}
