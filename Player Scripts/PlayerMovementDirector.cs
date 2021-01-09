using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementDirector : MonoBehaviour
{
    [Tooltip("The time in seconds that the movement takes to complete")][SerializeField] float movementTime = 0.5f;

    [SerializeField] GameObject sceneMovingObject = null;

    [Header("Public player movement fields")]
    public float xMovementPerMove = 2f, zMovementPerMove = 2f;
    public float xAmountMoved, zAmountMoved;
    public float xAmountToMove, zAmountToMove;
    public bool playerMoving;
    public Vector3 startingPosition;
    public Vector3 targetPosition;

    [Header("Player Movement Audio fields")]
    [Tooltip("The audio played when the player moves")] [SerializeField] AudioClip movementAudio = null;
    [Tooltip("The volume for player movment")] [SerializeField] float movementAudioVolume = 0.8f;


    float zDistCameraPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the starting position of the player
        startingPosition = transform.localPosition;
        targetPosition = transform.localPosition;

        // Find camera distance from player (only z as that's all we're changing)
        zDistCameraPlayer = sceneMovingObject.transform.localPosition.z - transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        // First we check if the targetPosition given is the same as where we are.
        // If it's not, we need to start moving

        if (Vector3.Distance(transform.localPosition, targetPosition) > 0.2)
        {
            playerMoving = true;
        }
        else
        {
            playerMoving = false;
            transform.position = targetPosition;
        }

        // If the player is not moving, we log where we currently are
        if (!playerMoving)
        {
            startingPosition = transform.localPosition;
            xAmountMoved = 0;
            zAmountMoved = 0;
            xAmountToMove = 0;
            zAmountToMove = 0;
        }

        // If the player should be moving, we start the moving process
        else
        {
            MovePlayer();
        }

        // Camera is always kept up with player through MoveCamera function
        MoveCamera();
    }

    private void MoveCamera()
    {
        float newCameraZPosition = transform.localPosition.z + zDistCameraPlayer;

        sceneMovingObject.transform.localPosition = new Vector3(sceneMovingObject.transform.localPosition.x,
            sceneMovingObject.transform.localPosition.y, newCameraZPosition);
    }

    public void MovePlayer()
    {
        // Note that this script is fed a target position by the PlayerController script,
        // and this script focuses on moving the enemy to that new position gradually -
        // it does not handle finding/calculating the target position itself. The PlayerController
        // instead handles that
        xAmountToMove = (targetPosition.x - startingPosition.x) * (Time.deltaTime / movementTime);
        zAmountToMove = (targetPosition.z - startingPosition.z) * (Time.deltaTime / movementTime);

        xAmountMoved += xAmountToMove;
        zAmountMoved += zAmountToMove;

        // todo improve audio, this doesn't add anything to the game at the moment
        // PlayMovementAudio();

        if (Mathf.Abs(xAmountMoved) < Mathf.Abs(xMovementPerMove) && zAmountMoved < zMovementPerMove)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + xAmountToMove, transform.localPosition.y, transform.localPosition.z + zAmountToMove);
        }

        else { transform.localPosition = targetPosition; }
   
    }

    private void PlayMovementAudio()
    {
        // Loads and plays movement audio (unless already playing)
        gameObject.GetComponent<AudioSource>().clip = movementAudio;

        gameObject.GetComponent<AudioSource>().volume = movementAudioVolume;

        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
