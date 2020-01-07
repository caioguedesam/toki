using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIInputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected PlayerControl player;

    public bool buttonHeldState = false;
    public bool pastHeldState;
    public bool buttonReleased = false;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonHeldState = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeldState = false;
    }

    protected virtual void Update()
    {
        if (buttonHeldState)
        {
            //pastHeldState = true;
        }   
        else
        {
            if(pastHeldState)
            {
                Debug.Log("button released");
                buttonReleased = true;
                //pastHeldState = false;
            }
            else
            {
                buttonReleased = false;
            }
        }

        pastHeldState = buttonHeldState;
    }
}
