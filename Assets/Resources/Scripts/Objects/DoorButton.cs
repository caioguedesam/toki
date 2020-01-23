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
    private bool isActivating = false;
    // Seconds for button timer
    public float buttonTimer = 3f;
    // Button timer variables
    private float buttonCurrentFrameTime = 0f;
    private float buttonPastFrameTime = 0f;
    [SerializeField]
    private float buttonStartTime = 0f;

    // Delay time for next toggle on button
    public float buttonToggleDelayTime = 0.3f;

    // Animator reference
    private Animator animator;

    
    [SerializeField]
    private bool isActive;
    public bool initialState;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Set initial current frame time to current time
        buttonCurrentFrameTime = Time.time;
        // And active state begins false
        isActive = false;
        initialState = isActive;

        // Setting timed parameter in animator
        animator.SetBool("isTimed", isTimed);
        // Setting animation slowdown or speedup for recovery of timed levers
        if(isTimed)
        {
            animator.SetFloat("RecoverTime", TimedAnimationMultiplier(buttonTimer, 1f));
        }
    }

    public void ResetObj()
    {
        Debug.Log("Resetting button");
        isActive = initialState;

        if(isTimed)
        {
            animator.SetTrigger("Reset");
            buttonStartTime = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Player pressed button
        if (collision.CompareTag("Player") && player.interactInput)
        {
            // Don't allow the player to press button during active state if timed
            if (isTimed && isActive)
                return;
            StartCoroutine(ToggleButton());
            player.CheckPressAnimation();
        }
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

    /// <summary>
    /// Activates all door objects from door button. Maybe generalize later for other objects?
    /// </summary>
    private void ActivateAllButtonObjects()
    {
        // Loop through every door and activate
        for(int i = 0; i < listOfDoors.Count; i++)
        {
            Door door = listOfDoors[i].GetComponent<Door>();
            door.ActivateDoor();
        }
        // Change button state every call to this method
        isActive = !isActive;
    }

    /// <summary>
    /// Coroutine to call when button is activated by player or clone.
    /// </summary>
    private IEnumerator ToggleButton()
    {
        if(!isActivating)
        {
            isActivating = true;
            Debug.Log("Pressed Button!");

            // Play lever pull/push animation depending on button state
            if (!isActive)
            {
                animator.SetTrigger("Pulled");
            }
            else if (!isTimed)
            {
                animator.SetTrigger("Pushed");
            }

            // Set button start time (important for rewind handling)
            buttonStartTime = buttonCurrentFrameTime;

            // If the button isn't timed, set delay
            if (!isTimed)
                yield return new WaitForSeconds(buttonToggleDelayTime);

            ActivateAllButtonObjects();

            isActivating = false;
        }
    }

    private float TimedAnimationMultiplier(float buttonTime, float animationTime)
    {
        return animationTime / buttonTime;
    }

    /// <summary>
    /// Checks if button should be retoggled based on rewinds every frame.
    /// </summary>
    private void CheckButtonToggle()
    {
        // Only perform the check if the button is timed and if the start button time is set
        if(isTimed && buttonStartTime != 0f)
        {
            // button is rewinding based on sign of current time - past time
            bool buttonIsRewinding = (buttonCurrentFrameTime - buttonPastFrameTime < 0);

            // Case 1: if button is rewinding, inactive and was previously active
            if(buttonIsRewinding && !isActive && buttonCurrentFrameTime < buttonStartTime + buttonTimer && buttonCurrentFrameTime > buttonStartTime)
            {
                ActivateAllButtonObjects();
            }
            // Case 2: if button is rewinding, active and was previously inactive
            else if (buttonIsRewinding && isActive && buttonCurrentFrameTime < buttonStartTime)
            {
                ActivateAllButtonObjects();
            }
            // Case 3: if button is not rewinding, active and needs to be deactivated
            else if(!buttonIsRewinding && isActive && buttonCurrentFrameTime > buttonStartTime + buttonTimer)
            {
                ActivateAllButtonObjects();
            }
        }
    }

    private void Update()
    {
        // Set past time as current time every frame, before updating current time
        buttonPastFrameTime = buttonCurrentFrameTime;

        // Update current time based on rewind state
        if (!TimeController.Instance.isRewindingTime)
        {
            buttonCurrentFrameTime += Time.deltaTime;
        }
        else
        {
            if(buttonCurrentFrameTime > 0 && !TimeController.Instance.playerIsFrozen)
            {
                buttonCurrentFrameTime -= Time.deltaTime;
            }
        }

        // Finally, check for needed toggles
        CheckButtonToggle();
    }
}
