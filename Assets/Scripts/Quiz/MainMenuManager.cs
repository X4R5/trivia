using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;

    [SerializeField] Button profileButton, marketButton, leaderboardButton, logoutButton;

    [SerializeField] List<Button> categoryButtons = new List<Button>();

    private void Awake()
    {
        foreach (var button in categoryButtons)
        {
            button.onClick.AddListener(() => LoadQuizPage(button.GetComponentInChildren<TMP_Text>().text));
        }
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        db = FirebaseFirestore.DefaultInstance;

        CheckIfUserLoggedIn();

        if(user == null) return;

        profileButton.onClick.AddListener(LoadProfileScene);
        marketButton.onClick.AddListener(LoadMarketScene);
        logoutButton.onClick.AddListener(Logout);
        leaderboardButton.onClick.AddListener(LoadLeaderboardScene);

        
        LoadPlayerPortrait();
    }

    private void LoadLeaderboardScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Leaderboard");
    }

    private void Logout()
    {
        auth.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene("FrontPage");
    }

    private void LoadMarketScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Market");
    }

    public void LoadQuizPage(string category)
    {
        Debug.Log("Loading " + category + " quiz page.");
        PlayerPrefs.SetString("SelectedCategory", category.ToLower());
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuizPage");
    }

    private void CheckIfUserLoggedIn()
    {
        if (user == null)
        {
            Debug.Log("User is not logged in.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("FrontPage");
        }
    }

    private async void LoadPlayerPortrait()
    {
        var allAvatars = Resources.LoadAll<Sprite>("AvatarPack");
        int currentAvatar = 0;

        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;

        if (isLoggedIn)
        {
            DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> user = snapshot.ToDictionary();
                snapshot.TryGetValue("profilePhoto", out currentAvatar);
            }
        }
        else
        {
            var json = Resources.Load<TextAsset>("User");
            var user = JsonUtility.FromJson<User>(json.text);
            currentAvatar = user.profilePhoto;
        }


        profileButton.GetComponent<Image>().sprite = allAvatars[currentAvatar];
    }

    public void LoadProfileScene()
    {
        ProfileManager.instance.OpenProfilePopup();
    }

}
