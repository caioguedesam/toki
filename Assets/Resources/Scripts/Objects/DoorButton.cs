using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    // Player object reference
    private PlayerControl player;
    // List of objects to activate with switch
    public List<GameObject> listOfDoors;
    // Is the button timed?
    public bool isTimed = false;
    public bool timerRunning { get; private set; } = false;
    // Seconds for button timer
    public float buttonTimer = 3f;
    // Button timer variables
    public float buttonCurrentTime;
    private float buttonStartTime = 0f;
    [SerializeField]
    private float buttonEndTime = 0f;
    [SerializeField]
    private float minButtonTime;

    // Delay time for next toggle on button
    public float buttonToggleDelayTime = 0.3f;
    // Is switch being activated?
    private bool isButtonActivating = false;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    private void Start()
    {
        minButtonTime = 0f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Player pressed button
        if (collision.CompareTag("Player") && player.interactInput)
            StartCoroutine(ToggleButton());
        // Clone pressed button
        else if(collision.CompareTag("TimeClone"))
        {
            Clone clone = collision.GetComponent<Clone>();
            if (clone.clonePositions[clone.posIndex-1].input.interactInput)
            {
                StartCoroutine(ToggleButton());
            }
        }
    }

    private void ActivateAllButtonObjects()
    {
        for(int i = 0; i < listOfDoors.Count; i++)
        {
            Door door = listOfDoors[i].GetComponent<Door>();
            door.ActivateDoor();
        }
    }

    private IEnumerator ToggleButton()
    {
        if(!isButtonActivating)
        {
            Debug.Log("Pressed Button!");
            isButtonActivating = true;

            ActivateAllButtonObjects();
            // If the button is timed, wait timer then re-toggle all objects
            if(isTimed)
            {
                /*timerRunning = true;
                buttonStartTime = Time.time;
                yield return new WaitForSeconds(buttonTimer);
                ActivateAllButtonObjects();
                timerRunning = false*/

                timerRunning = true;
                buttonStartTime = Time.time;

                while (buttonCurrentTime < buttonTimer) {
                    yield return null;
                }
                ActivateAllButtonObjects();
                buttonEndTime = Time.time;
                timerRunning = false;
            }
            else
                yield return new WaitForSeconds(buttonToggleDelayTime);

            isButtonActivating = false;
        }
    }

    // HOLY SHIT THIS IS SO BAD REWORK THIS PLEASE
    private void CalculateButtonCurrentTime()
    {
        /*if (!timerRunning && !TimeController.Instance.isRewindingTime)
        {
            if (buttonEndTime != 0 && Time.time - buttonEndTime <= TimeController.Instance.rewindSeconds)
            {
                Debug.Log(Time.time - buttonEndTime);
                buttonCurrentTime += Time.deltaTime;
                minButtonTime = Mathf.Max(minButtonTime, buttonCurrentTime - TimeController.Instance.rewindSeconds);
                Debug.Log("minbuttontime = " + minButtonTime);
            }
            else
            {
                buttonCurrentTime = 0f;
                //minButtonTime = 0f;
            }
            
        }*/
        if(!timerRunning)
        {
            buttonCurrentTime = 0f;
        }
        else if(!TimeController.Instance.isRewindingTime && buttonStartTime != 0 && buttonCurrentTime < buttonTimer)
        {
            //buttonCurrentTime = Time.time - buttonStartTime;
            buttonCurrentTime += Time.deltaTime;
            minButtonTime = Mathf.Max(minButtonTime, buttonCurrentTime - TimeController.Instance.rewindSeconds);
        }
        else if(TimeController.Instance.isRewindingTime)
        {
            //buttonCurrentTime = Mathf.Clamp(buttonCurrentTime - (Time.time - TimeController.Instance.rewindStartTime), 0f, buttonTimer);
            //Debug.Log("minbuttontime = " + minButtonTime);
            buttonCurrentTime = Mathf.Clamp(buttonCurrentTime - Time.deltaTime, minButtonTime, buttonTimer);
        }
    }

    private void Update()
    {
        CalculateButtonCurrentTime();
        //if(buttonCurrentTime != 0)
            //Debug.Log(buttonCurrentTime);
    }
}
