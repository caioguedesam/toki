using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIInputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected PlayerControl player;

    protected bool buttonHeldState = false;
    public bool currentHeldState { get; protected set; } = false;
    public bool pastHeldState { get; protected set; }
    public bool buttonReleased { get; protected set; } = false;

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
        pastHeldState = currentHeldState;
        currentHeldState = buttonHeldState;

        if (!currentHeldState && pastHeldState)
            buttonReleased = true;
        else
            buttonReleased = false;
    }
}
