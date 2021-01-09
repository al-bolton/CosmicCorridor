using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTracker : MonoBehaviour
{
    [Header("Related Game Objects")]
    [Tooltip("The audio conductor of the scene")] [SerializeField] GameObject conductor = null;

    [Header("Materials to be used")]
    [Tooltip("The usual material to be used")] [SerializeField] Material normalMaterial = null;
    [Tooltip("The material to be used when a mistake is made")] [SerializeField] Material mistakeMaterial = null;
    [Tooltip("The material to be used when a successful move is made")] [SerializeField] Material movementMaterial = null;

    public float currentScale;

    // Update is called once per frame
    void Update()
    {
        BeatScaleProcessing();

        ColourProcessing();
    }

    private void ColourProcessing()
    {
        // Function to change the material of the game object depending on
        // whether a mistake or movement has been made
        if (conductor.GetComponent<Conductor>().mistakeMade)
        {
            gameObject.GetComponent<MeshRenderer>().material = mistakeMaterial;
        }

        else if (conductor.GetComponent<Conductor>().actionMade)
        {
            gameObject.GetComponent<MeshRenderer>().material = movementMaterial;
        }

        else { gameObject.GetComponent<MeshRenderer>().material = normalMaterial; }
    }

    private void BeatScaleProcessing()
    {
        // Function to change the scale of the game object with the beat
        currentScale = conductor.GetComponent<Conductor>().songPositionInBeats % 1;

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}
