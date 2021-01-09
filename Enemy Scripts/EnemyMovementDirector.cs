using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementDirector : MonoBehaviour
{
    [Header("Public enemy movement fields")]
    [Tooltip("The time in seconds that the movement takes to complete")] [SerializeField] float movementTime = 0.5f;

    public float xMovementPerMove = 2f, zMovementPerMove = 2f;
    public float xAmountMoved, zAmountMoved;
    public float xAmountToMove, zAmountToMove;
    public bool enemyMoving;
    public Vector3 startingPosition;
    public Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Get the starting position of the enemy
        // COULD BE CAUSE FOR A BUG HERE WITH INITIAL MOVEMENT OF ENEMY
        // todo testing required here
        startingPosition = transform.localPosition;
        targetPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // First we check if the targetPosition given is the same as where we are.
        // If it's not, we need to start moving

        if (Vector3.Distance(transform.localPosition, targetPosition) > 0.2)
        {
            enemyMoving = true;
        }
        else
        {
            enemyMoving = false;
            transform.localPosition = targetPosition;
        }

        // If the enemy is not moving, we log where we currently are
        if (!enemyMoving)
        {
            startingPosition = transform.localPosition;
            xAmountMoved = 0;
            zAmountMoved = 0;
            xAmountToMove = 0;
            zAmountToMove = 0;
        }

        // If the enemy should be moving, we start the moving process
        else
        {
            MoveEnemy();
        }
    }

    private void MoveEnemy()
    {
        // Functions similarly to player movement - this script is fed a target
        // position by the Enemy Controller script, and this script focuses on moving
        // the enemy to that new position gradually - it does not handle finding the
        // target position itself
        xAmountToMove = (targetPosition.x - startingPosition.x) * (Time.deltaTime / movementTime);
        zAmountToMove = (targetPosition.z - startingPosition.z) * (Time.deltaTime / movementTime);

        xAmountMoved += xAmountToMove;
        zAmountMoved += zAmountToMove;


        if (Mathf.Abs(xAmountMoved) < Mathf.Abs(xMovementPerMove) && Mathf.Abs(zAmountMoved) < Mathf.Abs(zMovementPerMove))
        {
            transform.localPosition = new Vector3(transform.localPosition.x + xAmountToMove, transform.localPosition.y, transform.localPosition.z + zAmountToMove);
        }

        else { transform.localPosition = targetPosition; }
    }
}
