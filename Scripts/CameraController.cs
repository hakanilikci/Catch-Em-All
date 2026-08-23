using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    void Start()
    {
        // Calculate the initial offset distance between the camera and the player
        offset = transform.position - player.transform.position;
    }
    void LateUpdate()
    {
        // LateUpdate is used for camera to ensure player has finished moving
        if (player != null)
        {
            // Maintain the same offset from the player as they move
            transform.position = player.transform.position + offset;
        }
    }
}