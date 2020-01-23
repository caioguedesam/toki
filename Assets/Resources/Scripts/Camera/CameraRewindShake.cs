using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRewindShake : MonoBehaviour
{
    // Is camera shaking?
    private bool isShaking = false;
    // Source of rewind shake (Time Controller this is attached to)
    private CinemachineImpulseSource impulseSource;
    // Camera that listens to shake
    public CinemachineImpulseListener cameraListener;

    // Shake maximum magnitude
    public float maxShakeMagnitude = 5f;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private IEnumerator Shake()
    {
        if(!isShaking)
        {
            // Stop other calls to this coroutine after first call
            isShaking = true;

            // Store origina gain from camera impulse listener
            float originalGain = cameraListener.m_Gain;

            // Update gain while time is rewinding up until max shake magnitude
            while(TimeController.Instance.isRewindingTime)
            {
                float currentRewindTime = Mathf.Clamp(Time.time - TimeController.Instance.rewindStartTime, 0f, TimeController.Instance.rewindSeconds);
                float currentRewindScale = Mathf.InverseLerp(0f, TimeController.Instance.rewindSeconds, currentRewindTime);

                cameraListener.m_Gain = originalGain * currentRewindScale * maxShakeMagnitude;

                yield return null;
            }

            // Restore original gain from camera impulse listener
            cameraListener.m_Gain = originalGain;
            // Allow new coroutine calls
            isShaking = false;
        }
    }

    private void Update()
    {
        if(TimeController.Instance.isRewindingTime)
        {
            // Send shake impulse
            impulseSource.GenerateImpulse();
            // Control shake values
            StartCoroutine(Shake());
        }
    }
}
