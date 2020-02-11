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
    [SerializeField] private float buttonCurrentFrameTime = 0f;
    private float buttonPastFrameTime = 0f;
    [SerializeField]
    private float buttonStartTime = 0f;

    // Delay time for next toggle on button
    public float buttonToggleDelayTime = 0.3f;

    // Animation time variable for handling time rewinding levers
    private float lastAnimTime = 0f;

    // Animator reference
    private Animator animator;

    
    [SerializeField]
    private bool isActive;
    public bool initialState;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.Normal;
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
                // Don't allow the player to press button during active state if timed
                if (isTimed && isActive)
                    return;
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

            Debug.Log("Regular ToggleButton");
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
                Debug.Log("CheckButtonToggle - Case1");
                ActivateAllButtonObjects();
            }
            // Case 2: if button is rewinding, active and was previously inactive
            else if (buttonIsRewinding && isActive && buttonCurrentFrameTime < buttonStartTime)
            {
                Debug.Log("CheckButtonToggle - Case2");
                ActivateAllButtonObjects();
            }
            // Case 3: if button is not rewinding, active and needs to be deactivated
            else if(!buttonIsRewinding && isActive && buttonCurrentFrameTime > buttonStartTime + buttonTimer)
            {
                Debug.Log("CheckButtonToggle - Case3");
                ActivateAllButtonObjects();
            }
        }
    }

    private void Update()
    {
        animator.enabled = !TimeController.Instance.playerIsFrozen;
        animator.SetBool("isActive", isActive);
        // Set past time as current time every frame, before updating current time
        buttonPastFrameTime = buttonCurrentFrameTime;

        // Update current time based on rewind state
        if (!TimeController.Instance.isRewindingTime)
        {
            buttonCurrentFrameTime += Time.deltaTime;
            animator.SetFloat("RewindMultiplier", 1f);
            // Storing last animation time when not time rewinding
            lastAnimTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
        else
        {
            animator.SetFloat("RewindMultiplier", -1f);
            animator.ResetTrigger("GoBack");

            if (buttonCurrentFrameTime > 0 && !TimeController.Instance.playerIsFrozen)
            {
                buttonCurrentFrameTime -= Time.deltaTime;
            }
        }

        // Handling animation rewind for timed buttons
        if(isTimed && TimeController.Instance.isRewindingTime)
        {
            animator.SetFloat("RecoverTime", TimedAnimationMultiplier(buttonTimer, -1f));

            // If animation reaches start, go back to previous one
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime - lastAnimTime >= 1)
            {
                //Debug.Log("animation time diff: " + (animator.GetCurrentAnimatorStateInfo(0).normalizedTime - lastAnimTime));
                animator.SetTrigger("GoBack");
                lastAnimTime = 0f;
            }
        }
        else
        {
            animator.SetFloat("RecoverTime", TimedAnimationMultiplier(buttonTimer, 1f));
        }

        // Finally, check for needed toggles
        CheckButtonToggle();
    }
}
