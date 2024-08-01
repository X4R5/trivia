using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore db;

    [SerializeField] int jokerPrice = 50, avatarPrice = 100;
    [SerializeField] List<Button> jokerButtons = new List<Button>();
    [SerializeField] List<Button> avatarButtons = new List<Button>();
    [SerializeField] List<int> boughtAvatars = new List<int>();
    [SerializeField] Button backToMenuButton;

    [SerializeField] TMP_Text pointsText;
    int points, currentAvatar;


    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        backToMenuButton.onClick.AddListener(BackToMenu);

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
        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;

        if (isLoggedIn)
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
        else
        {
            var json = Resources.Load<TextAsset>("User");
            var user = JsonUtility.FromJson<User>(json.text);

            if (user.points >= jokerPrice)
            {
                user.points -= jokerPrice;
                pointsText.text = user.points.ToString();

                switch (jokerId)
                {
                    case 0:
                        user.bombJokerCount++;
                        break;
                    case 1:
                        user.doubleAnswerJokerCount++;
                        break;
                    case 2:
                        user.skipQuestionJokerCount++;
                        break;
                }

                var jsonUser = JsonUtility.ToJson(user, true);

                var filePath = Path.Combine(Application.dataPath, "Resources/User.json");

                System.IO.File.WriteAllText(filePath, jsonUser);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.ImportAsset("Assets/Resources/User.json", UnityEditor.ImportAssetOptions.ForceUpdate);
#endif

                UpdatePointsText();
            }
        }
    }

    private void UpdatePointsText()
    {
        Debug.Log("Points: " + points);
        pointsText.text = points.ToString() + " Points";
    }

    private async void LoadMarket()
    {
        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;

        if (isLoggedIn)
        {
            await db.Collection("users").Document(auth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
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
                }
                else
                {
                    Debug.Log("No market data found.");
                }
            });
        }
        else
        {
            var json = Resources.Load<TextAsset>("User");
            var user = JsonUtility.FromJson<User>(json.text);

            points = user.points;
            currentAvatar = user.profilePhoto;
            boughtAvatars = user.boughtAvatars;
        }

        UpdatePointsText();
        SetupAvatarButtons();

        if(FakeLoadingScreen.instance != null) FakeLoadingScreen.instance.SpeedUpLoading();
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
        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;

        if (points >= avatarPrice)
        {
            points -= avatarPrice;
            boughtAvatars.Add(avatarId);
            currentAvatar = avatarId;

            if (isLoggedIn)
            {
                DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
                docRef.UpdateAsync("points", FieldValue.Increment(-avatarPrice));
                docRef.UpdateAsync("boughtAvatars", FieldValue.ArrayUnion(avatarId));
                docRef.UpdateAsync("profilePhoto", avatarId);
            }
            else
            {
                var json = Resources.Load<TextAsset>("User");
                var user = JsonUtility.FromJson<User>(json.text);

                user.points -= avatarPrice;
                user.boughtAvatars.Add(avatarId);
                user.profilePhoto = avatarId;

                var jsonUser = JsonUtility.ToJson(user, true);
                var filePath = Path.Combine(Application.dataPath, "Resources/User.json");
                System.IO.File.WriteAllText(filePath, jsonUser);

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.ImportAsset("Assets/Resources/User.json", UnityEditor.ImportAssetOptions.ForceUpdate);
#endif
            }

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

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
