using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] int scorePerTime = 100;
    [SerializeField] float scoreTimerInterval = 5f;
    [SerializeField] int scorePerForwardMovement = 50;

    public int score;
    Text scoreText;
    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        TimeBasedScore();
    }

    private void TimeBasedScore()
    {
        timer += Time.deltaTime;
        if (timer > scoreTimerInterval)
        {
            score += scorePerTime;
            scoreText.text = score.ToString();
            timer %= scoreTimerInterval;
        }
    }

    public void ScoreMovementForward()
    {
        score += scorePerForwardMovement;
        scoreText.text = score.ToString();
    }

    public void ScoreEnemyDeath(int scorePerEnemyHit)
    {
        score += scorePerEnemyHit;
        scoreText.text = score.ToString();
    }
}
