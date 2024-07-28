using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionTimeManager : MonoBehaviour
{
    public static QuestionTimeManager instance;

    [SerializeField] private float timeToAnswer = 10f;
    [SerializeField] Image progressBar;
    [SerializeField] private TMP_Text timeText;

    private float timeRemaining;
    bool wait = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        timeRemaining = timeToAnswer;
        timeText.text = timeToAnswer.ToString();
    }

    private void Update()
    {
        if(wait) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            progressBar.fillAmount = timeRemaining / timeToAnswer;
            timeText.text = Mathf.Round(timeRemaining).ToString();
        }
        else
        {
            wait = true;
            var quizManager = FindFirstObjectByType<QuizManager>();
            quizManager.TimeUp();
        }
    }

    public void ResetTimer()
    {
        timeRemaining = timeToAnswer;
        progressBar.fillAmount = 1;
        timeText.text = timeToAnswer.ToString();
        wait = false;
    }
}
