using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCollisionHandler : MonoBehaviour
{
    [Tooltip("The despawn delay after death in seconds")] [SerializeField] float enemyDespawnTimer = 5f;
    [Tooltip("The load delay after death in seconds")] [SerializeField] AudioClip deathAudio = null;

    [Tooltip("The score given for killing this enemy")] [SerializeField] int enemyDeathScore = 300;

    public bool enemyAlive = true;

    BoxCollider enemyCollider;

    GameObject score = null;

    // Start is called before the first frame update
    void Start()
    {
        enemyCollider = gameObject.GetComponent<BoxCollider>();

        FindScore();
    }

    private void FindScore()
    {
        // We find the scoreboard in the scene which we will tell to update the
        // score when the enemy dies
        if (score == null)
        {
            score = GameObject.FindGameObjectWithTag("Score");
        }
    }

    private void OnTriggerEnter(Collider collisionObject)
    {
        // Stop doing anything on trigger if the enemy dies
        if (!enemyAlive) { return; }

        // Only cause trigger to do something if it's a player bullet - enemies
        // can't die to other enemy bullets
        if (collisionObject.name == "Player Bullet(Clone)")
        {
            StartDeathSequence();
        }
        
    }

    private void StartDeathSequence()
    {
        // Sends message of enemy death to other components such as controller
        // and bullet generator
        SendMessage("EnemyDeath");

        // Plays Death sound
        PlayDeathAudio();

        // Set enemyAlive to false to stop further death sequence trigger
        enemyAlive = false;

        // Turn off collider so bullets can pass over enemy corpse
        enemyCollider.enabled = false;

        // Start timer for enemy despawn
        Invoke("EnemyDespawn", enemyDespawnTimer);

        // Send score update for killing enemy
        score.GetComponent<ScoreTracker>().ScoreEnemyDeath(enemyDeathScore);

    }

    private void PlayDeathAudio()
    {
        // Loads and plays death audio (unless already playing)
        gameObject.GetComponent<AudioSource>().clip = deathAudio;

        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void EnemyDespawn()
    {
        // Simple function to despawn enemy
        Destroy(gameObject);
    }

}
