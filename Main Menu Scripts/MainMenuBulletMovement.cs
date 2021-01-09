using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBulletMovement : MonoBehaviour
{
    [Header("Related Game Objects")]
    [Tooltip("The audio conductor of the scene")] [SerializeField] public static GameObject conductor = null;

    [Header("Bullet Movement parameters")]
    [Tooltip("The distance the bullet will move every beat")] [SerializeField] Vector3 bulletMovementPerTick = new Vector3(0,0,-2f);
    [Tooltip("The time taken for the bullet to move every beat")] [SerializeField] float movementTime = 0.5f;
    [Tooltip("Bullet range (in tiles)")] [SerializeField] int bulletRange = 10;

    // Public fields used to track beat for when bullet moves, and whether it has already moved
    public float currentBeatMark;
    // public bool bulletMoved;

    // Fields used in bullet movement to make it move gradually
    float zAmountToMove;
    float zAmountMoved;
    Vector3 movementStartPosition = new Vector3(0, 0, 0);

    // Field used in bullet correction to stay in middle of tile
    bool bulletCorrected;

    // Field used in bullet range calculation
    Vector3 bulletStartingPosition = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        // Set initial position. Movement starting position will change later,
        // bulletStarting (used for range calculation) will not
        movementStartPosition = transform.position;
        bulletStartingPosition = transform.position;

        // Find the conductor in the scene
        FindConductor();
    }

    private void FindConductor()
    {
        // Because bullets are instantiated, function used to find conductor
        // in the scene for beat tracking

        if (conductor == null)
        {
            conductor = GameObject.FindGameObjectWithTag("Conductor");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Process the bullet's movement
        ProcessBeatMovement();

        // Correct the bullet if it hasn't been already
        if (!bulletCorrected)
        {
            CorrectBullet();
        }

        // If the bullet has moved past it's range, we destroy it
        BulletRangeCheck();
    }

    private void ProcessBeatMovement()
    {
        // Get the current beat mark from the conductor
        currentBeatMark = conductor.GetComponent<MainMenuConductor>().songPositionInBeats % 1;

        // If we're on beat, we move the bullet
        if (currentBeatMark < movementTime || currentBeatMark > 1 - movementTime)
        {
            MoveBullet();
        }

        // Bullet is reset outside of the beat mark
        else
        {
            zAmountMoved = 0;

            movementStartPosition = transform.position;
        }
    }

    private void MoveBullet()
    {
        // Calculate how much we want the bullet to move this beat
        zAmountToMove = bulletMovementPerTick.z * (Time.deltaTime / movementTime);

        // Add this to the total moved for the overall beat movement
        zAmountMoved += zAmountToMove;

        // While we haven't hit the distance we want this beat, we move the bullet
        if (Mathf.Abs(zAmountMoved) < Mathf.Abs(bulletMovementPerTick.z))
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + zAmountToMove);
        }

        // Once we've hit the required distance, we finalise our movement
        else
        {
            // Here we reset the position subtley so that it stays in the
            // centre of the tile
            transform.position = movementStartPosition + bulletMovementPerTick;
        }
    }

    private void CorrectBullet()
    {
        // todo this needs fixing, doesn't work properly

        float modulusBulletZPosition = transform.position.z % 2;

        // Function to correct the bullet if it is not in the centre of a tile
        if (modulusBulletZPosition % 2 != 1)
        {
            float newBulletZPosition = 0;

            if (modulusBulletZPosition % 2 > 1)
            {
                newBulletZPosition = transform.position.z - (modulusBulletZPosition - 1);
            }

            else if (modulusBulletZPosition < 1)
            {
                newBulletZPosition = transform.position.z + (1 - modulusBulletZPosition);
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, newBulletZPosition);

            bulletCorrected = true;
        }
    }

    private void BulletRangeCheck()
    {
        // Checks if the bullet has passed its range, and if it has it is destroyed
        // Remember we multiple range by 2 to convert from tiles to distance
        if(Vector3.Distance(bulletStartingPosition, transform.position) > bulletRange * 2)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Bullet is deleted from the game when it hits a trigger
        // (Player, other bullet or box of deletion)
        Destroy(gameObject);
    }
}
