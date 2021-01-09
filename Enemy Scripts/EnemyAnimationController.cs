using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator enemyAnimator = null;
    EnemyMovementDirector enemyMovementDirector = null;
    EnemyController enemyController = null;

    // Start is called before the first frame update
    void Start()
    {
        // Setup the enemy animator
        enemyAnimator = gameObject.GetComponent<Animator>();

        // Setup the enemy movement director
        enemyMovementDirector = gameObject.GetComponent<EnemyMovementDirector>();

        // Setup the enemy controller
        enemyController = gameObject.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set enemy X Speed equal to the amount we're going to move
        enemyAnimator.SetFloat("X Speed", enemyMovementDirector.xAmountToMove);

        // Set enemy Z Speed equal to the amount we're moving in z-direction
        enemyAnimator.SetFloat("Z Speed", enemyMovementDirector.zAmountToMove);

        // Set whether the enemy is alive from the enemy controller
        enemyAnimator.SetBool("Alive", enemyController.enemyAlive);
    }
}
