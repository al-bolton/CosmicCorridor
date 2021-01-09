using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [Tooltip("The load delay after death in seconds")] [SerializeField] float levelLoadDelay = 1f;
    [Tooltip("The audio played upon player death")] [SerializeField] AudioClip deathAudio = null;
    [Tooltip("The volume for player death")][SerializeField] float deathAudioVolume = 0.8f;
    [Tooltip("The scene death menu")] [SerializeField] GameObject deathMenu = null;

    bool playerAlive = true;

    private void OnTriggerEnter(Collider other)
    {
        // Stop doing anything on trigger if the player dies
        if (!playerAlive) { return; }

        StartDeathSequence();
    }

    private void StartDeathSequence()
    {
        // Sends message of player death to other components such as controller
        // which stops them from doing anything such as taking action input
        SendMessage("PlayerDeath");

        // Plays Death sound
        PlayDeathAudio();

        // Reloads the scene
        Invoke("ShowDeathMenu", levelLoadDelay);

        playerAlive = false;

    }

    private void PlayDeathAudio()
    {
        // Loads and plays death audio (unless already playing)
        gameObject.GetComponent<AudioSource>().Stop();

        gameObject.GetComponent<AudioSource>().clip = deathAudio;

        gameObject.GetComponent<AudioSource>().volume = deathAudioVolume;

        gameObject.GetComponent<AudioSource>().Play();
        
    }

    private void ShowDeathMenu() // String referenced in Invoke, needs to be chnaged there if changed
    {
        // Uses the function on the Death Menu to set up for the game
        deathMenu.GetComponent<DeathScreenManager>().DeathMenuStartup();
    }

}
