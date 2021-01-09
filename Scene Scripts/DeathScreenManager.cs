using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DeathScreenManager : MonoBehaviour
{
    [Header("Related Game Objects")]
    [Tooltip("Score tracker scene object")][SerializeField] GameObject scoreTracker = null;
    [Tooltip("Score display on the menu")] [SerializeField] GameObject scoreDisplay = null;
    [Tooltip("Beat tracker scene object")] [SerializeField] GameObject beatTracker = null;

    [Tooltip("This should be the default button that the player starts selected")] [SerializeField] GameObject defaultButton = null;

    public void DeathMenuStartup()
    {
        // Get the final score
        TextMeshProUGUI scoreText = scoreDisplay.GetComponent<TextMeshProUGUI>();
        float scoreValue = scoreTracker.GetComponent<ScoreTracker>().score;

        scoreText.text = (scoreValue.ToString());

        // Show the menu
        gameObject.SetActive(true);

        // Turn off beat trackers
        beatTracker.SetActive(false);

        // Set the default seleted button
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
