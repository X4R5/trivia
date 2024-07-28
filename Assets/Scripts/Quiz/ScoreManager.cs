using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] TMP_Text quizScoreText;
    private int quizScore = 0;

    private void Awake()
    {
        SingletonThisGO();
    }

    private void Start()
    {
        quizScoreText.text = "Score: " + quizScore.ToString();
    }

    public void AddQuizScore(int scoreToAdd)
    {
        quizScore += scoreToAdd;
        quizScoreText.text = "Score: " + quizScore.ToString();
    }

    public void RemoveQuizScore(int scoreToRemove)
    {
        quizScore -= scoreToRemove;
        quizScoreText.text = "Score: " + quizScore.ToString();
    }

    public int GetQuizScore()
    {
        return quizScore;
    }


    private void SingletonThisGO()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
