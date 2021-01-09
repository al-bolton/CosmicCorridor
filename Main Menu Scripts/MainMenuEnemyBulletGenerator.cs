using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnemyBulletGenerator : MonoBehaviour
{
    [Header("Related Game Objects")]
    [Tooltip("The audio conductor of the scene")]public GameObject conductor = null;
    [Tooltip("The bullet prefab to use")] [SerializeField] GameObject bullet = null;
    [Tooltip("The parent object where the bullets are placed")] public GameObject bulletParent = null;
    Animator enemyAnimator = null;

    [Header("Bullet Generation Parameters")]
    [Tooltip("How many beats pass per bullet generated")] [SerializeField] float beatsPerBullet = 2f;
    [Tooltip("How many beats before bullets are generated")] [SerializeField] float initialBeatOffset = 2f;
    [Tooltip("Chance per generation a bullet is actually made")] [SerializeField] float chanceOfBullet = 0.5f;
    [Tooltip("Distance in front of enemy bullet is generated")] [SerializeField] Vector3 initialBulletDistanceOffset = new Vector3(0, 0, 0);
    [SerializeField] float beatThreshold = 0.02f;

    [Header("Generated Bullet Rotation")]
    [Tooltip("Bullet rotation in relation to parent object")] [SerializeField] Vector3 bulletRotation = new Vector3 (0, 0, 0);

    public float currentBeatMark;
    public float currentBeatMarkWithOffset;
    public bool bulletInstantiated;
    public bool bulletFired;
    public bool offsetHit = false;

    bool enemyAlive = true;


    // Start is called before the first frame update
    void Start()
    {
        // Setup the player animator
        enemyAnimator = gameObject.GetComponent<Animator>();

        // Find the required game objects upon instantiation
        FindConductor();
        FindBulletParent();
    }

    private void FindConductor()
    {
        // Because enemies are instantiated, function used to find conductor in the scene

        if (conductor == null)
        {
            conductor = GameObject.FindGameObjectWithTag("Conductor");
        }
    }

    private void FindBulletParent()
    {
        // Because enemies are instantiated, function used to find bullet parent object in the scene

        if (bulletParent == null)
        {
            bulletParent = GameObject.FindGameObjectWithTag("BulletParent");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Don't generate bullets if the enemy has died
        if (!enemyAlive) { return; }

        BulletGeneration();

        // For animation
        enemyAnimator.SetBool("Bullet Fired", bulletFired);
    }

    private void BulletGeneration()
    {
        
        // Before the initial offset is hit, we do nothing

        if (!offsetHit)
        {
            currentBeatMark = conductor.GetComponent<MainMenuConductor>().songPositionInBeats;

            if (currentBeatMark > initialBeatOffset) { offsetHit = true; }

            return;

        }

        // Get the current beat mark from the conductor, dependent on the beats per bullet
        currentBeatMarkWithOffset = (conductor.GetComponent<MainMenuConductor>().songPositionInBeats - initialBeatOffset) % beatsPerBullet;

        // If we're on the right beat, we want to check if we should make a bullet
        if (currentBeatMarkWithOffset < beatThreshold || currentBeatMarkWithOffset > beatsPerBullet - beatThreshold)
        {
            // Only create a bullet if we haven't already done so this beat
            if (!bulletInstantiated)
            {
                if (BulletGenerationChance())
                {
                    Vector3 bulletPosition = transform.position + initialBulletDistanceOffset;

                    Instantiate(bullet, bulletPosition, Quaternion.Euler(bulletRotation), bulletParent.transform);

                    bulletFired = true;
                }    

                bulletInstantiated = true;

            }

        }

        // Bullet is reset outside of the beat mark
        else
        {
            bulletInstantiated = false;
            bulletFired = false;
        }
    }

    private bool BulletGenerationChance()
    {
        // This function rolls to see if we actually shoot a bullet on this go
        float chanceRoll = UnityEngine.Random.Range(0f, 1f);

        if (chanceRoll <= chanceOfBullet)
        {
            return true;
        }

        else { return false; }
    }

    private void EnemyDeath()
    {
        // Function is not called in this script, but is used in SendMessage in
        // other scripts. If changed, needs to also be changed there
        enemyAlive = false;
    }
}
