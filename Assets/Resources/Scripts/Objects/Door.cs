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

    /*public void SetInitialState()
    {
        //initialState = gameObject.activeSelf;

        // Setting initial state in animator
    }*/

    public void ActivateDoor()
    {
        Debug.Log("Toggling door!");
        //gameObject.SetActive(!gameObject.activeSelf);
        animator.SetTrigger("Toggled");
        impulseSource.GenerateImpulse();
    }

    public void ResetObj()
    {
        Debug.Log("Resetting " + name + " to " + initialState);
        //gameObject.SetActive(initialState);
        animator.SetTrigger("Reset");
    }
}
