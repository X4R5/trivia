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

    [SerializeField] Button profileButton;

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

        profileButton.onClick.AddListener(LoadProfileScene);

        CheckIfUserLoggedIn();
        LoadPlayerPortrait();
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("SignIn");
        }
    }

    private void LoadPlayerPortrait()
    {
        var allAvatars = Resources.LoadAll<Sprite>("AvatarPack");
        int avatarIndex = PlayerPrefs.GetInt("profilePhoto");

        profileButton.GetComponent<Image>().sprite = allAvatars[avatarIndex];
    }

    public void LoadProfileScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Profile");
    }

}
