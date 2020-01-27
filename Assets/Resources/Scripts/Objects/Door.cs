using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Door : MonoBehaviour
{
    public bool initialState;
    // Animator reference
    private Animator animator;

    // Source of camera shake when toggling
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        animator.SetBool("InitialState", initialState);
    }

    public void ActivateDoor()
    {
        Debug.Log("Toggling door at " + Time.time);
        animator.SetTrigger("Toggled");
        impulseSource.GenerateImpulse();
    }

    public void ResetObj()
    {
        animator.SetTrigger("Reset");
    }
}
