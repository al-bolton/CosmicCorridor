using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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
    [Tooltip("The scene corridor generator")] [SerializeField] GameObject sceneCorridorGenerator = null;

    // Parameters needed in code
    public float currentBeatMark;
    public bool enemyAlive = true;
    public bool movementAttempted;

    Vector3 enemyStartingPosition;
    float targetX, targetZ;

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
        FindCorridorGenerator();
    }

    private void FindConductor()
    {
        // Because enemies are instantiated, function used to find conductor in the scene

        if (sceneConductor == null)
        {
            sceneConductor = GameObject.FindGameObjectWithTag("Conductor");
        }
    }

    private void FindCorridorGenerator()
    {
        // Because enemies are instantiated, function used to find corridor generator in the scene

        if (sceneCorridorGenerator == null)
        {
            sceneCorridorGenerator = GameObject.FindGameObjectWithTag("CorridorGen");
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
        currentBeatMark = sceneConductor.GetComponent<Conductor>().songPositionInBeats % beatsPerMovement;

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

            CorridorGenerator corridorGenerator = sceneCorridorGenerator.GetComponent<CorridorGenerator>();

            // Target position (moving forward, backward, left or right in that order)
            // is based on the roll from the random generator
            switch (movementRandomDirection)
            {
                case 0:
                    targetX = transform.position.x;
                    targetZ = transform.position.z - zMovementPerTick;
                    break;
                case 1:
                    targetX = transform.position.x;
                    targetZ = transform.position.z + zMovementPerTick;
                    break;
                case 2:
                    targetX = Mathf.Clamp(transform.position.x - xMovementPerTick,
                    enemyStartingPosition.x - xRange, enemyStartingPosition.x + xRange);
                    targetZ = transform.position.z;
                    break;
                default:
                    targetX = Mathf.Clamp(transform.position.x + xMovementPerTick,
                    enemyStartingPosition.x - xRange, enemyStartingPosition.x + xRange);
                    targetZ = transform.position.z;
                    break;
            }

            // First, want to prevent the enemy moving back further than the corridor actually is
            if ((((int)targetZ - 1) / 2) > (corridorGenerator.obstacleListTracker.Count - 1))
            {
                targetZ = transform.position.z;
            }

            // Now we check the corridor generator to see if there's an obstacle on
            // that tile. If there isn't, we set the enemy target position to its new position
            if (corridorGenerator.obstacleListTracker[((int)targetZ - 1) / 2][((int)targetX - 1) / 2] == 0)
            {
                // Change to new position
                enemyMoveDirector.targetPosition = new Vector3(targetX, transform.position.y, targetZ);
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
