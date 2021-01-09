using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnemyGenerator : MonoBehaviour
{
    [Header("Related Game Objects")]
    [Tooltip("The audio conductor of the scene")] [SerializeField] GameObject conductor = null;
    [Tooltip("The enemy prefab to use")] [SerializeField] GameObject enemyPrefab = null;
    [Tooltip("The parent object where the bullets are placed")] [SerializeField] GameObject enemyParent = null;

    [Header("Enemy Generation Parameters")]
    [Tooltip("How many beats pass per enemy generated")] [SerializeField] float beatsPerEnemy = 2f;
    [Tooltip("How many beats before enemies are generated")] [SerializeField] float initialBeatOffset = 2f;
    [Tooltip("Chance per generation a enemy is actually made")] [SerializeField] float chanceOfEnemy = 0.5f;
    [Tooltip("Distance in front of generator that the enemy is generated")] [SerializeField] Vector3 initialEnemyDistanceOffset = new Vector3(0, 0, 0);
    [SerializeField] float beatThreshold = 0.02f;
    [Tooltip("Enemy rotation in relation to parent object")] [SerializeField] Vector3 enemyRotation = new Vector3(0, 0, 0);

    public float currentBeatMarkWithOffset;
    public float currentBeatMark;
    public bool enemyInstantiated;
    public bool offsetHit = false;

    // Update is called once per frame
    void Update()
    {
        EnemyGeneration();
    }

    private void EnemyGeneration()
    {
        // Before the initial offset is hit, we do nothing

        if (!offsetHit)
        {
            currentBeatMark = conductor.GetComponent<MainMenuConductor>().songPositionInBeats;

            if (currentBeatMark > initialBeatOffset) { offsetHit = true; }

            return;

        }

        // Get the current beat mark from the conductor, dependent on the beats per enemy
        currentBeatMarkWithOffset = (conductor.GetComponent<MainMenuConductor>().songPositionInBeats - initialBeatOffset) % beatsPerEnemy;

        // If we're on the right beat, we want to check if we should make an enemy
        if (currentBeatMarkWithOffset < beatThreshold || currentBeatMarkWithOffset > beatsPerEnemy - beatThreshold)
        {
            // Only create an enemy if we haven't already done so this beat
            if (!enemyInstantiated)
            {
                if (EnemyGenerationChance())
                {
                    Vector3 enemyPosition = transform.position + initialEnemyDistanceOffset;

                    Instantiate(enemyPrefab, enemyPosition, Quaternion.Euler(enemyRotation), enemyParent.transform);
                }

                enemyInstantiated = true;

            }

        }

        // Enemy generation is reset outside of the beat mark
        else
        {
            enemyInstantiated = false;
        }
    }

    private bool EnemyGenerationChance()
    {
        // Roll to see if we're going to instantiate an enemy on this beat mark,
        // dependent on chance parameter set
        float chanceRoll = UnityEngine.Random.Range(0f, 1f);

        if (chanceRoll <= chanceOfEnemy)
        {
            return true;
        }

        else { return false; }
    }
}
