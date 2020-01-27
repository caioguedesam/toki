using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewindTouch : MonoBehaviour
{
    PlayerControl player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            player.rewindInput = Input.GetMouseButton(0);
            player.stoppedRewindInput = Input.GetMouseButtonUp(0);
        }
    }
}
