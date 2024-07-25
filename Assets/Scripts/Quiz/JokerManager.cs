using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JokerManager : MonoBehaviour
{
    [SerializeField] Button bombJokerButton, doubleAnswerJokerButton, skipQuestionJokerButton;

    private QuizManager quizManager;

    private void Awake()
    {
        quizManager = FindObjectOfType<QuizManager>();

        bombJokerButton.onClick.AddListener(UseBombJoker);
        doubleAnswerJokerButton.onClick.AddListener(UseDoubleAnswerJoker);
        skipQuestionJokerButton.onClick.AddListener(UseSkipQuestionJoker);
    }

    private void UseSkipQuestionJoker()
    {
        if (quizManager != null)
        {
            quizManager.SkipCurrentQuestion();
        }
    }

    private void UseDoubleAnswerJoker()
    {
        if (quizManager != null)
        {
            quizManager.ApplyDoubleAnswerJoker();
        }
    }

    private void UseBombJoker()
    {
        if (quizManager != null)
        {
            quizManager.ApplyBombJoker();
        }
    }
}
