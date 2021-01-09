using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is not to be used during normal gameplay. It pans the camera forward,
// and is only useful for videos or for the main menu

public class CameraPan : MonoBehaviour
{
    [Tooltip("Camera pan in x-direction in ms^-1")][SerializeField] float cameraXPanSpeed = 0f;
    [Tooltip("Camera pan in y-direction in ms^-1")] [SerializeField] float cameraYPanSpeed = 0f;
    [Tooltip("Camera pan in z-direction in ms^-1")] [SerializeField] float cameraZPanSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        // This function moves the camera gradually
        float xAmountToMove = cameraXPanSpeed * Time.deltaTime;
        float yAmountToMove = cameraYPanSpeed * Time.deltaTime;
        float zAmountToMove = cameraZPanSpeed * Time.deltaTime;

        transform.localPosition = new Vector3(transform.localPosition.x + xAmountToMove,
            transform.localPosition.y + yAmountToMove,
            transform.localPosition.z + zAmountToMove);
    }
}
