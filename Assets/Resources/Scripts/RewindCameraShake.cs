using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindCameraShake : MonoBehaviour
{
    private bool isShaking = false;
    [Range(0f, 5f)]
    public float shakeMagnitude = 5f;

    private IEnumerator Shake()
    {
        if(!isShaking)
        {
            isShaking = true;
            Debug.Log("Started shake corot");
            Vector3 originalPosition = transform.localPosition;

            float currentShake;

            while (TimeController.Instance.rewindingTime)
            {
                float currentTime = Time.time - TimeController.Instance.rewindStartTime;
                currentShake = Mathf.Lerp(0f, shakeMagnitude, currentTime);

                float posX = Random.Range(-1f, 1f) * currentShake;
                float posY = Random.Range(-1f, 1f) * currentShake;
                Debug.Log("current shake: " + currentShake);

                transform.localPosition = new Vector3(posX, posY, originalPosition.z);

                yield return null;
            }

            transform.localPosition = originalPosition;
            Debug.Log("Ended shake corot");
            isShaking = false;
        }
    }

    private void Update()
    {
        if(TimeController.Instance.rewindingTime)
        {
            StartCoroutine(Shake());
        }
    }
}
