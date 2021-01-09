using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnemyController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [Tooltip("The distance a player can move in x-direction every tick")] [SerializeField] public float xMovementPerTick = 2f;
    [Tooltip("The distance a player can move in z-direction every tick")] [SerializeField] public float zMovementPerTick = 2f;
    [Tooltip("The range of x-direction to be allowed either side of the character")] [SerializeField] float xRange = 4f;
    [Tooltip("The amount of tiles either side of the character")] [SerializeField] int xRangeTiles = 2;

    float tileWidth;

    [Header("Enemy Movement Parameters")]
    [Tooltip("How many beats pass per bullet generated")] [SerializeField] float beatsPerMovement = 2f;
    [Tooltip("Probability of enemy moving on each beat (out of 1)")] [SerializeField] float chanceToMove = 0.3f;
    [SerializeField] float beatThreshold = 0.02f;

    [Header("Related Game Objects")]
    [Tooltip("The conductor for the scene")] GameObject sceneConductor;

    // Parameters needed in code
    public float currentBeatMark;
    public bool enemyAlive = true;
    public bool movementAttempted;

    Vector3 enemyStartingPosition;

    EnemyMovementDirector enemyMoveDirector;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate tile width based on range and how many tiles wide corridor is
        tileWidth = xRange / xRangeTiles;

        // Set starting position of enemy
        CalculateStartingPosition();

        // Find necessary game objects & components
        enemyMoveDirector = GetComponent<EnemyMovementDirector>();
        FindConductor();
    }

    private void FindConductor()
    {
        // Because enemies are instantiated, function used to find conductor in the scene

        if (sceneConductor == null)
        {
            sceneConductor = GameObject.FindGameObjectWithTag("Conductor");
        }
    }

    private void CalculateStartingPosition()
    {
        // This all works (including limiting to the movment range) on the
        // assumption the generator makes the enemy in the middle of the corridor

        // Randomise the starting x offset from the center (in tiles)
        int startingOffset = UnityEngine.Random.Range(-xRangeTiles, xRangeTiles);

        // Take record of the starting position
        enemyStartingPosition = transform.position;

        // Now move the enemy
        transform.position = new Vector3(transform.position.x + startingOffset * tileWidth,
            transform.position.y,
            transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovementControl();
    }

    private void EnemyMovementControl()
    {
        // Get the current beat mark from the conductor, dependent on the beats per movement
        currentBeatMark = sceneConductor.GetComponent<MainMenuConductor>().songPositionInBeats % beatsPerMovement;

        // If we're on the right beat, we want to check if we should try to move
        if (currentBeatMark < beatThreshold || currentBeatMark > beatsPerMovement - beatThreshold)
        {
            // Only attempt movement if we haven't already done so this beat
            if (!movementAttempted)
            {
                MovementAttempt();

                movementAttempted = true;

            }

        }

        // Movement attempt is reset outside of the beat mark
        else
        {
            movementAttempted = false;
        }
    }

    private void MovementAttempt()
    {
        if (!enemyAlive) { return; }

        // Randomise if we're going to make a movement on this beat
        float movementRandomRoll = UnityEngine.Random.Range(0f, 1f);

        // Then if we did hit the chance to move, we randomise what direction we're going to move in
        if (movementRandomRoll <= chanceToMove)
        {
            int movementRandomDirection = UnityEngine.Random.Range(0, 4);

            // If we hit 0, we move forwards (remember forward is -ve, towards player)
            if (movementRandomDirection == 0)
            {
                enemyMoveDirector.targetPosition = new Vector3(transform.position.x,
                transform.position.y,
                transform.position.z - zMovementPerTick);
            }

            // If we hit 1, we move backwards
            else if (movementRandomDirection == 1)
            {
                enemyMoveDirector.targetPosition = new Vector3(transform.position.x,
                transform.position.y,
                transform.position.z + zMovementPerTick);
            }

            // If we hit 2, we move left.
            // Bear in mind we need to keep within our range here, so we use clamps
            else if (movementRandomDirection == 2)
            {
                float xClampedPosition = Mathf.Clamp(transform.position.x - xMovementPerTick,
                    enemyStartingPosition.x - xRange, enemyStartingPosition.x + xRange);

                enemyMoveDirector.targetPosition = new Vector3(xClampedPosition,
                transform.position.y,
                transform.position.z);
            }

            // If we hit 3, we move right.
            // Bear in mind we need to keep within our range here, so we use clamps
            // This could be else but I'm worried to change it
            else if (movementRandomDirection == 3)
            {
                float xClampedPosition = Mathf.Clamp(transform.position.x + xMovementPerTick,
                    enemyStartingPosition.x - xRange, enemyStartingPosition.x + xRange);

                enemyMoveDirector.targetPosition = new Vector3(xClampedPosition,
                transform.position.y,
                transform.position.z);
            }
        }
    }

    private void EnemyDeath()
    {
        // Function is not called in this script, but is used in SendMessage in
        // other scripts. If changed, needs to also be changed there
        enemyAlive = false;
    }
}
