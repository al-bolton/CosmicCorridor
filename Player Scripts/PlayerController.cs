using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// IMPORTANT NOTE FOR THIS SCRIPT

// To help understand this script, it's useful to note that for each action (movement,
// shooting, reload, etc.), we have two functions - one to take the input, and one
// to actually commit the action. This is required as we still want to take input while
// off-beat, to see if the player makes a mistake - however, we don't want to actually
// act on that input. So, off-beat, only the input functions are called. on-beat, the
// action functions are then called by the conductor instead

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [Tooltip("The distance a player can move in x-direction every tick")][SerializeField] public float xMovementPerTick = 2f;
    [Tooltip("The distance a player can move in z-direction every tick")] [SerializeField] public float zMovementPerTick = 2f;
    [Tooltip("The starting x-position of the character model (should be starting in the middle of the corridor)")] [SerializeField] float characterStartingXPosition = 5f;
    [Tooltip("The range of x-direction to be allowed either side of the character")] [SerializeField] float xRange = 4f;
    [Tooltip("The amount the controller joystick must be pressed to count as an input")] [SerializeField] float controllerSensitivity = 0.3f;

    [Header("Related Game Objects")]
    // [Tooltip("The audio conductor of the scene")] [SerializeField] GameObject conductor = null;
    // [Tooltip("The Main Camera for the scene")] [SerializeField] GameObject sceneMovementObject = null;
    [Tooltip("The audio played upon player shooting")] [SerializeField] AudioClip shootingAudio = null;
    [Tooltip("The audio played upon player shooting")] [SerializeField] AudioClip reloadAudio = null;
    [Tooltip("The volume for player shooting")] [SerializeField] float shootingAudioVolume = 0.3f;
    [Tooltip("The volume for player shooting")] [SerializeField] float reloadAudioVolume = 0.3f;
    [Tooltip("The scene corridor generator")] [SerializeField] GameObject sceneCorridorGenerator = null;


    [Header("Player Shooting fields")]
    [Tooltip("The prefab used for player bullets")] [SerializeField] GameObject playerBullet;
    [Tooltip("The parent object to be used for the player bullets")] [SerializeField] Transform playerBulletParent;
    [Tooltip("Bullet rotation in relation to parent object")] [SerializeField] Vector3 bulletRotation = new Vector3(0, 0, 0);
    [Tooltip("Distance in front of enemy bullet is generated")] [SerializeField] Vector3 initialBulletDistanceOffset = new Vector3(0, 0, 0);
    public bool bulletLoaded = true;

    public float xMovement, zMovement;
    public bool playerAlive = true;
    public bool isAxisInUse;
    public bool isShooting;
    public bool isReloading;
    PlayerMovementDirector playerMoveDirector;

    GameObject score = null;

    // Start is called before the first frame update
    void Start()
    {
        playerMoveDirector = gameObject.GetComponent<PlayerMovementDirector>();

        FindScore();
    }

    private void FindScore()
    {
        // Find the scoreboard in the scene so we can update it when the player moves
        if (score == null)
        {
            score = GameObject.FindGameObjectWithTag("Score");
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessRawMovementAxisInput();

        ProcessShootingInput();
    }

    private void ProcessRawMovementAxisInput()
    {
        // This function essentially means input is only taken once

        // If there is input, we set it so that there has been an input, and
        // this is used in the ProcessMovementInput() function
        if (CrossPlatformInputManager.GetAxisRaw("Horizontal") != 0 || CrossPlatformInputManager.GetAxisRaw("Vertical") != 0)
        {
            if (isAxisInUse == false)
            {
                isAxisInUse = true;
            }
        }

        // If there's no joystick/axis input, we reset
        if (CrossPlatformInputManager.GetAxisRaw("Horizontal") == 0 && CrossPlatformInputManager.GetAxisRaw("Vertical") == 0)
        {
            isAxisInUse = false;
        }
    }

    public void PlayerDeath()
    {
        // Function called if the player dies, stops all movement
        playerAlive = false;
    }

    public bool ProcessMovement()
    {
        // If the player is dead, we do not take input
        if (!playerAlive) { return false; }

        // If the player is alive
        ProcessMovementInput();

        if (xMovement != 0 || zMovement != 0)
        {
            CalculateNewPosition();

            // Returns true if a movement was made
            return true;
        }
        return false;
    }

    public bool ProcessMovementInput()
    {
        if (!isAxisInUse)
        {
            // Movement input in x-direction
            xMovement = CrossPlatformInputManager.GetAxisRaw("Horizontal");

            // Movement input in z-direction
            zMovement = CrossPlatformInputManager.GetAxisRaw("Vertical");
        }

        else { xMovement = 0; zMovement = 0; }

        // Detection for controller, round to values of -1, 0 and 1
        if (xMovement > controllerSensitivity) { xMovement = 1f; }
        else if (xMovement < -controllerSensitivity) { xMovement = -1f; }
        else { xMovement = 0f; }

        if (zMovement > controllerSensitivity) { zMovement = 1f; }

        // Do not allow player to move backwards
        else { zMovement = 0f; }

        // Function returns true if there was an input, otherwise it returns fals
        if (xMovement !=0 || zMovement != 0) { return true; }

        return false;
    }

    private void CalculateNewPosition()
    {
        // Movement calculation in x-direction
        float rawNewXPos = transform.localPosition.x + xMovement * xMovementPerTick;
        float clampedXPos = Mathf.Clamp(rawNewXPos, characterStartingXPosition - xRange, characterStartingXPosition + xRange);

        // Movement calculation in z-direction
        // Do not allow z movement if already moved in x, x is preferred axis
        if (xMovement != 0) { zMovement = 0f; }

        float rawNewZPos = transform.localPosition.z + zMovement * zMovementPerTick;

        // Check the obstacle tracker to see if we can move to where we want

        CorridorGenerator corridorGenerator = sceneCorridorGenerator.GetComponent<CorridorGenerator>();

        // todo remove this - only here for debug purposes
        // print("z: " + rawNewZPos + "\nx: " + clampedXPos);

        if (corridorGenerator.obstacleListTracker[((int)rawNewZPos - 1) / 2][((int)clampedXPos - 1) / 2] == 0)
        {
            // Change to new position
            playerMoveDirector.targetPosition = new Vector3(clampedXPos, transform.localPosition.y, rawNewZPos);

            // Score the player if they moved forward
            if (zMovement > Mathf.Epsilon)
            {
                score.GetComponent<ScoreTracker>().ScoreMovementForward();
            }
        }
    }

    public bool ProcessShootingInput()
    {
        // This function essentially means shooting input is only taken once

        // If there is input, we set it so that there has been an input, and
        // this is used in the ProcessShooting() function
        if (CrossPlatformInputManager.GetAxisRaw("Shoot") != 0)
        {
            if (isShooting == false)
            {
                isShooting = true;
                return true;
            }

            return false;
        }

        // If there's no joystick/axis input, we reset
        else
        {
            isShooting = false;

            return false;
        }
    }

    public bool ProcessShooting()
    {
        // Only works if the player isn't dead
        if (!playerAlive) { return false; }

        // If we do in fact want to shoot as the method returns true,
        // only then do we instantiate a bullet
        if (ProcessShootingInput())
        {
            if(bulletLoaded)
            {
                // Instantiate a player bullet on screen
                Instantiate(playerBullet, transform.position + initialBulletDistanceOffset, Quaternion.Euler(bulletRotation), playerBulletParent);
                bulletLoaded = false;

                // Plays Death sound
                PlayShootingAudio();

                //  We have now fired, so want to tell conductor an action was made
                return true;
            }
        }

        return false;
    }

    public bool ProcessReloadingInput()
    {
        // This function essentially means reloading input is only taken once

        // If there is input, we set it so that there has been an input, and
        // this is used in the ProcessReloading() function
        if (CrossPlatformInputManager.GetAxisRaw("Reload") != 0)
        {
            if (isReloading == false)
            {
                isReloading = true;
                return true;
            }

            return false;
        }

        // If there's no joystick/axis input, we reset
        else
        {
            isReloading = false;

            return false;
        }
    }

    public bool ProcessReloading()
    {
        // Only works if the player isn't dead
        if (!playerAlive) { return false; }

        // If we do in fact want to reload as the method returns true,
        // only then do we load a bullet
        if (ProcessReloadingInput())
        {
            if (!bulletLoaded)
            {
                // Instantiate a player bullet on screen
                bulletLoaded = true;

                // Plays Death sound
                PlayReloadAudio();

                //  We have now reloaded, so want to tell conductor an action was made
                return true;
            }
        }

        return false;
    }

    private void PlayShootingAudio()
    {
        // Loads and plays shooting audio (unless already playing)
        gameObject.GetComponent<AudioSource>().clip = shootingAudio;

        gameObject.GetComponent<AudioSource>().volume = shootingAudioVolume;

        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void PlayReloadAudio()
    {
        // Loads and plays death audio (unless already playing)
        gameObject.GetComponent<AudioSource>().clip = reloadAudio;

        gameObject.GetComponent<AudioSource>().volume = reloadAudioVolume;

        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
