using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [Header("Related Game Objects")]
    [Tooltip("The audio conductor of the scene")] [SerializeField] GameObject conductor = null;
    [Tooltip("The bullet prefab to use")] [SerializeField] GameObject bullet = null;
    [Tooltip("The parent object where the bullets are placed")] [SerializeField] GameObject bulletParent = null;

    [Header("Bullet Generation Parameters")]
    [Tooltip("How many beats pass per bullet generated")] [SerializeField] float beatsPerBullet = 2f;
    [Tooltip("x-direction range either side of generator where bullets can be generated (in panels)")] [SerializeField] int xRange = 2;
    [Tooltip("Panel x width (int)")] [SerializeField] int panelWidth = 2;
    [Tooltip("Minimum number of bullets generated per tick (min of 1)")] [SerializeField] int minBullets = 1;
    [Tooltip("Maximum number of bullets generated per tick (max of 4)")] [SerializeField] int maxBullets = 1;

    [Header("Generated Bullet Rotation")]
    [Tooltip("Bullet x-rotation in relation to parent object")] [SerializeField] float bulletXRotation = 0f;
    [Tooltip("Bullet y-rotation in relation to parent object")] [SerializeField] float bulletYRotation = 0f;
    [Tooltip("Bullet z-rotation in relation to parent object")] [SerializeField] float bulletZRotation = 0f;

    [SerializeField] float beatThreshold = 0.02f;
    public float currentBeatMark;
    public bool bulletInstantiated;

    // Update is called once per frame
    void Update()
    {
        BulletGeneration();
    }

    private void BulletGeneration()
    {
        // Get the current beat mark from the conductor, dependent on the beats per bullet
        currentBeatMark = conductor.GetComponent<Conductor>().songPositionInBeats % beatsPerBullet;

        // If we're on the right beat, we want to check if we should make a bullet
        if (currentBeatMark < beatThreshold || currentBeatMark > beatsPerBullet - beatThreshold)
        {
            // Only create a bullet if we haven't already done so this beat
            if (!bulletInstantiated)
            {
                BulletGenerationPreparation();

                bulletInstantiated = true;

            }

        }

        // Bullet is reset outside of the beat mark
        else
        {
            bulletInstantiated = false;
        }
    }

    private void BulletGenerationPreparation()
    {
        // First we make our list of possible values using the xRange variable

        List<int> randomXRange = new List<int>();

        for (int i = 0; i <= 2 * xRange; i++)
        {
            randomXRange.Add(i);
        }

        // We see how many bullets we're going to instantiate this tick

        int bulletsThisTick = UnityEngine.Random.Range(minBullets, maxBullets + 1);

        // Then we take away random values from this list so that we're only left
        // with the amount of bullets we want to generate at. This is done instead
        // of using the random functionso that bullets are not instantiated
        // at the same spot

        int listValuesToRemove = (2 * xRange) + 1 - bulletsThisTick;

        for (int j = 0; j < listValuesToRemove; j++)
        {
            int indexToRemove = UnityEngine.Random.Range(0, randomXRange.Count);
            randomXRange.RemoveAt(indexToRemove);
        }

        // For the values that remain after removal, we instantiate a bullet
        // at that x value

        for (int i = 0; i < randomXRange.Count; i++)
        {
            InstantiateBullet(randomXRange[i] * panelWidth);
        }

    }

    private void InstantiateBullet(int bulletRelativeXValue)
    {
        // Set bullet transform
        Vector3 newBulletTransform = new Vector3(transform.position.x - (xRange * panelWidth) + bulletRelativeXValue,
            transform.position.y,
            transform.position.z);

        // Bullet is instantiated
        Instantiate(bullet, newBulletTransform, Quaternion.Euler(bulletXRotation, bulletYRotation, bulletZRotation), bulletParent.transform);
    }
}
