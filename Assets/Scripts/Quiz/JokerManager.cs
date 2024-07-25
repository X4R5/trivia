using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using Firebase.Extensions;
using TMPro;

public class JokerManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore db;

    [SerializeField] Button bombJokerButton, doubleAnswerJokerButton, skipQuestionJokerButton;

    private QuizManager quizManager;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        SetJokerCounts();

        quizManager = FindObjectOfType<QuizManager>();

        bombJokerButton.onClick.AddListener(UseBombJoker);
        doubleAnswerJokerButton.onClick.AddListener(UseDoubleAnswerJoker);
        skipQuestionJokerButton.onClick.AddListener(UseSkipQuestionJoker);
    }

    private void SetJokerCounts()
    {
        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;

            if (snapshot.Exists)
            {
                Dictionary<string, object> user = snapshot.ToDictionary();
                snapshot.TryGetValue("bombJokerCount", out int bombJokerCount);
                snapshot.TryGetValue("doubleAnswerJokerCount", out int doubleAnswerJokerCount);
                snapshot.TryGetValue("skipQuestionJokerCount", out int skipQuestionJokerCount);

                bombJokerButton.GetComponentInChildren<TMP_Text>().text = bombJokerCount.ToString();
                doubleAnswerJokerButton.GetComponentInChildren<TMP_Text>().text = doubleAnswerJokerCount.ToString();
                skipQuestionJokerButton.GetComponentInChildren<TMP_Text>().text = skipQuestionJokerCount.ToString();

                bombJokerButton.interactable = bombJokerCount > 0;
                doubleAnswerJokerButton.interactable = doubleAnswerJokerCount > 0;
                skipQuestionJokerButton.interactable = skipQuestionJokerCount > 0;
            }
        });
    }

    private void UseSkipQuestionJoker()
    {
        if (quizManager != null)
        {
            quizManager.SkipCurrentQuestion();

            skipQuestionJokerButton.GetComponentInChildren<TMP_Text>().text = (int.Parse(skipQuestionJokerButton.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
            skipQuestionJokerButton.interactable = false;

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            docRef.UpdateAsync("skipQuestionJokerCount", FieldValue.Increment(-1));
        }
    }

    private void UseDoubleAnswerJoker()
    {
        if (quizManager != null)
        {
            quizManager.ApplyDoubleAnswerJoker();

            doubleAnswerJokerButton.GetComponentInChildren<TMP_Text>().text = (int.Parse(doubleAnswerJokerButton.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
            doubleAnswerJokerButton.interactable = false;

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            docRef.UpdateAsync("doubleAnswerJokerCount", FieldValue.Increment(-1));
        }
    }

    private void UseBombJoker()
    {
        if (quizManager != null)
        {
            quizManager.ApplyBombJoker();

            bombJokerButton.GetComponentInChildren<TMP_Text>().text = (int.Parse(bombJokerButton.GetComponentInChildren<TMP_Text>().text) - 1).ToString();
            bombJokerButton.interactable = false;

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            docRef.UpdateAsync("bombJokerCount", FieldValue.Increment(-1));
        }
    }
}
