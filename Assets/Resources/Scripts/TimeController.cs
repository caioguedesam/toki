using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeController : MonoBehaviour
{
    // Singleton instance
    public static TimeController Instance { get; private set; }

    // Is time rewinding?
    public bool isRewindingTime = false;
    // How much time to rewind
    public int rewindSeconds = 2;
    // Has rewind just stopped?
    public bool stoppedRewind = false;
    // Time current rewind started
    public float rewindStartTime = 0f;
    // Time current rewind ended
    public float rewindEndTime = 0f;
    // Total rewind time of last rewind
    public float lastRewindTime = 0f;
    // Rewind time slow/speed effect scale
    public AnimationCurve rewindTimeScaleCurve;

    // Coroutine controllers
    private bool isCheckingRewindTime = false;
    private bool isChangingTimeScale = false;

    // Original deltaTime
    private float originalFixedDeltaTime;

    public int stoppedRewindTimeFrame = 1;
    // Player reference
    public PlayerControl player;
    public bool playerIsFrozen;
    // List of player time clones present in scene
    public List<GameObject> cloneList;
    // Maximum number of player time clones
    public int maxCloneLimit = 1;
    // Clone spawn cooldown
    public int cloneRespawnSeconds = 1;

    private void Awake()
    {
        // If there's nothing in the instance property, assign TimeController information to object.
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If there is, then there's another TimeController object. Destroy this one.
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        cloneList = new List<GameObject>();
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    /// <summary>
    /// Checking if time is rewinding every frame. Also handles minor things regarding time rewinding.
    /// </summary>
    private void CheckRewind()
    {
        isRewindingTime = player.rewindInput;
        // If rewind input, start rewind
        if (isRewindingTime)
        {
            StartCoroutine(GetRewindTime());
        }

        // Change time scale when rewinding
        ChangeRewindTimeScale();

        // Detect when rewind is stopped
        stoppedRewind = player.stoppedRewindInput;
    }

    private IEnumerator GetRewindTime()
    {
        if(!isCheckingRewindTime)
        {
            // Stopping other coroutine calls from doing this
            isCheckingRewindTime = true;

            // Recording rewind start time
            rewindStartTime = Time.time;

            // Waiting for rewind to end
            while (isRewindingTime || player.GetComponent<RewindPlayer>().isFrozen)
            {
                yield return null;
            }

            // Recording rewind end time and total time
            rewindEndTime = Time.time;
            lastRewindTime = Mathf.Min(rewindEndTime - rewindStartTime, rewindSeconds);


            // Allowing for next rewind to be recorded
            isCheckingRewindTime = false;
        }
    }

    /// <summary>
    /// Method to change time scale based on time curve, to add feedback to player rewinding time.
    /// </summary>
    public void ChangeRewindTimeScale()
    {
        if(isRewindingTime)
        {
            // Evaluate given time from rewind start point in time curve
            Time.timeScale = rewindTimeScaleCurve.Evaluate(Time.time - rewindStartTime);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Adds a new player position to the position list.
    /// </summary>
    /// <param name="gameObj">Object to store position from.</param>
    /// <param name="posArray">List to add position in.</param>
    public void AddPosition(GameObject gameObj, List<PlayerTimePosition> posArray)
    {
        // Making new position to store in list
        PlayerTimePosition pos = new PlayerTimePosition();
        pos.position = gameObj.transform.position;
        pos.time = Time.time;
        pos.input = new TimePositionInput();
        

        // Sprite handling
        SpriteRenderer renderer = gameObj.GetComponentInChildren<SpriteRenderer>();
        pos.sprite = renderer.sprite;
        pos.facingRight = gameObj.GetComponent<PlayerControl>().facingRight;

        // If it's the player adding a new position, also store inputs
        if(gameObj.CompareTag("Player"))
        {
            pos.input.SetInput();
        }

        posArray.Add(pos);

        // Continuously removing all positions outside time threshold
        posArray.RemoveAll(x => x.time <= (Time.time - rewindSeconds));
    }

    /// <summary>
    /// Adds a new box collider object position to the position list. Used for simple stuff like rewinding pillars/levers.
    /// </summary>
    public void AddPosition(GameObject gameObj, List<BoxColliderTimePosition> posArray)
    {
        BoxColliderTimePosition pos = new BoxColliderTimePosition();
        pos.time = Time.time;
        pos.sprite = gameObj.GetComponent<SpriteRenderer>().sprite;
        pos.colliderSize = gameObj.GetComponent<BoxCollider2D>().size;

        posArray.Add(pos);
        // Continuously removing all positions outside time threshold
        posArray.RemoveAll(x => x.time <= (Time.time - rewindSeconds));
    }

    /// <summary>
    /// Adds a new throwable position to the position list.
    /// </summary>
    public void AddPosition(Throwable obj, List<ThrowableTimePosition> posArray)
    {
        // Making new position to store in list
        ThrowableTimePosition pos = new ThrowableTimePosition();
        pos.position = obj.transform.position;
        pos.time = Time.time;
        pos.isBeingHeld = obj.isBeingHeld;
        pos.moveAmount = obj.moveAmount;

        posArray.Add(pos);

        // Continuously removing all positions outside time threshold
        posArray.RemoveAll(x => x.time <= (Time.time - rewindSeconds));

    }

    public void AddPosition(HoldObject obj, List<HoldObjectPosition> posArray)
    {
        // Making new position to store in list
        HoldObjectPosition pos = new HoldObjectPosition(obj.transform.position, obj.GetComponent<Rigidbody2D>().velocity, obj.transform.parent, Time.time);

        posArray.Add(pos);

        // Continuously removing all positions outside time threshold
        posArray.RemoveAll(x => x.time <= (Time.time - rewindSeconds));
    }

    /// <summary>
    /// Adds a time clone to the controller's clone list.
    /// </summary>
    /// <param name="clone"></param>
    public void AddClone(GameObject clone)
    {
        cloneList.Add(clone);
    }

    /// <summary>
    /// Activates all time clones from the clone list.
    /// </summary>
    public void ActivateAllClones()
    {
        for(int i = 0; i < cloneList.Count; i++)
        {
            GameObject clone = cloneList[i];
            if (!clone.activeSelf)
                clone.SetActive(true);
        }
    }

    /// <summary>
    /// Checks if there are positions assigned to all the clones
    /// </summary>
    public bool CheckSetClones()
    {
        for(int i = 0; i < cloneList.Count; i++)
        {
            if (cloneList[i].GetComponent<Clone>().clonePositions.Count == 0)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Destroys all time clones from the time clone list.
    /// </summary>
    public void DestroyAllClones()
    {
        for (int i = 0; i < cloneList.Count; i++)
        {
            Destroy(cloneList[i]);
            cloneList.RemoveAt(i);
        }
    }

    /// <summary>
    /// Checks for input to destroy time clones in game.
    /// </summary>
    public void CheckDestroyClones()
    {
        if (player.timeClearInput && cloneList.Count > 0)
            DestroyAllClones();
    }

    private void Update()
    {
        // If player reference is missing, fetch it back. Happens when changing levels
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        }

        CheckRewind();
        CheckDestroyClones();
        playerIsFrozen = player.GetComponent<RewindPlayer>().isFrozen;
    }

    // FIX THIS! REWIND ON SCREEN TOUCH
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

[System.Serializable]
public class PlayerTimePosition
{
    public Vector3 position;
    public float time;
    public TimePositionInput input;
    public Sprite sprite;
    public bool facingRight;
}

[System.Serializable]
public class BoxColliderTimePosition
{
    public float time;
    public Sprite sprite;
    public Vector2 colliderSize;
}

[System.Serializable]
public class TimePositionInput
{
    public bool jumpInput;
    public bool timeClearInput;
    public bool interactInput;
    public bool rewindInput;

    /// <summary>
    /// Sets the position inputs based on current player input.
    /// </summary>
    public void SetInput()
    {
        PlayerControl player = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();

        jumpInput = player.jumpInput;
        timeClearInput = player.timeClearInput;
        interactInput = player.interactInput;
        rewindInput = player.rewindInput;
    }
}
