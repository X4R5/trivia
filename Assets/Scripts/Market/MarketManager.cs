using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore db;

    [SerializeField] int jokerPrice = 50, avatarPrice = 100;
    [SerializeField] List<Button> jokerButtons = new List<Button>();
    [SerializeField] List<Button> avatarButtons = new List<Button>();
    [SerializeField] List<int> boughtAvatars = new List<int>();

    [SerializeField] TMP_Text pointsText;
    int points, currentAvatar;


    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        LoadMarket();

        SetJokerButtons();
    }

    private void SetJokerButtons()
    {
        for (int i = 0; i < jokerButtons.Count; i++)
        {
            int jokerId = i;
            jokerButtons[jokerId].onClick.AddListener(() => BuyJoker(jokerId));
        }
    }

    private void BuyJoker(int jokerId)
    {
        if (points >= jokerPrice)
        {
            points -= jokerPrice;
            pointsText.text = points.ToString();

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            docRef.UpdateAsync("points", FieldValue.Increment(-jokerPrice));

            switch (jokerId)
            {
                case 0:
                    docRef.UpdateAsync("bombJokerCount", FieldValue.Increment(1));
                    break;
                case 1:
                    docRef.UpdateAsync("doubleAnswerJokerCount", FieldValue.Increment(1));
                    break;
                case 2:
                    docRef.UpdateAsync("skipQuestionJokerCount", FieldValue.Increment(1));
                    break;
            }

            UpdatePointsText();
        }
    }

    private void UpdatePointsText()
    {
        pointsText.text = points.ToString() + " Points";
    }

    private void LoadMarket()
    {
        db.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading market: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                points = snapshot.GetValue<int>("points");
                currentAvatar = snapshot.GetValue<int>("profilePhoto");
                boughtAvatars = snapshot.GetValue<List<int>>("boughtAvatars");

                UpdatePointsText();
                SetupAvatarButtons();
            }
            else
            {
                Debug.Log("No market data found.");
            }
        });
    }

    private void SetupAvatarButtons()
    {
        for (int i = 0; i < avatarButtons.Count; i++)
        {
            int avatarId = i;

            if (boughtAvatars.Contains(avatarId))
            {
                avatarButtons[avatarId].onClick.AddListener(() => ChangeAvatar(avatarId));
                avatarButtons[avatarId].GetComponentInChildren<TMP_Text>().text = "";
            }
            else
            {
                avatarButtons[avatarId].onClick.AddListener(() => BuyAvatar(avatarId));
                avatarButtons[avatarId].GetComponentInChildren<TMP_Text>().text = avatarPrice.ToString() + " Points";
            }
        }

        avatarButtons[currentAvatar].GetComponentInChildren<TMP_Text>().text = "Selected";
    }

    private void BuyAvatar(int avatarId)
    {
        if (points >= avatarPrice)
        {
            points -= avatarPrice;
            boughtAvatars.Add(avatarId);
            currentAvatar = avatarId;

            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            docRef.UpdateAsync("points", FieldValue.Increment(-avatarPrice));
            docRef.UpdateAsync("boughtAvatars", FieldValue.ArrayUnion(avatarId));
            docRef.UpdateAsync("profilePhoto", avatarId);


            UpdatePointsText();
            SetupAvatarButtons();
        }
    }

    private void ChangeAvatar(int avatarId)
    {
        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        docRef.UpdateAsync("profilePhoto", avatarId);
        currentAvatar = avatarId;

        avatarButtons[avatarId].GetComponentInChildren<TMP_Text>().text = "Selected";

        SetupAvatarButtons();
    }
}
