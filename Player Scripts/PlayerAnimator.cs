using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator playerAnimator = null;
    PlayerMovementDirector playerMovementDirector = null;
    PlayerController playerController = null;
    GameObject sceneConductor = null;

    // Start is called before the first frame update
    void Start()
    {
        // Setup the player animator
        playerAnimator = gameObject.GetComponent<Animator>();

        // Setup the player movement director
        playerMovementDirector = gameObject.GetComponent<PlayerMovementDirector>();

        // Setup the player controller
        playerController = gameObject.GetComponent<PlayerController>();

        // Find the conductor in the scene
        sceneConductor = GameObject.FindGameObjectWithTag("Conductor");
    }

    // Update is called once per frame
    void Update()
    {
        // Set player X Speed equal to the amount we're going to move (so if left, our speed is -ve)
        playerAnimator.SetFloat("X Speed", playerMovementDirector.xAmountToMove);

        // Set player Z Speed equal to the amount we're moving in z-direction
        playerAnimator.SetFloat("Z Speed", playerMovementDirector.zAmountToMove);

        // Set whether the player is alive from the player controller
        playerAnimator.SetBool("Alive", playerController.playerAlive);

        // Set whether the player is shooting from the conductor
        playerAnimator.SetBool("Bullet Fired", sceneConductor.GetComponent<Conductor>().shotFired);
    }

}
