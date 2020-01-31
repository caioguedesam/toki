using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to implement infinite scrolling parallax background for the game.
/// Inspired by this tutorial: https://youtu.be/zit45k6CUMk, with a small extension for
/// background elements that move on their own.
/// Caio Guedes, 2020.
/// </summary>
public class Parallax : MonoBehaviour
{
    // Image's start position and length
    private float startPos, length;
    // Camera
    public GameObject cameraObject;
    // Parallax multiplier
    public float parallaxEffect;
    // Self moving background variables
    public bool isMoving = false;
    public float moveSpeed = 0f;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Calculate reach for checking how much of the image is still left
        float reach = cameraObject.transform.position.x * (1 - parallaxEffect);
        // Calculating how much of the image was already moved
        float distance = cameraObject.transform.position.x * parallaxEffect;

        // Update position of image according to camera movement
        transform.position = new Vector2(startPos + distance, transform.position.y);
        // If the object is self moving, update startposition every new frame
        if(isMoving)
        {
            startPos += moveSpeed;
        }

        // Checking if camera already crossed image border
        if (reach > startPos + length) startPos += length;
        else if (reach < startPos - length) startPos -= length;
    }
}
