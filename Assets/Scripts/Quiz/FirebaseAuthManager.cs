using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    private FirebaseFirestore db;


    [SerializeField] TMPro.TMP_InputField usernameField, mailField, passwordField;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (db == null)
        {
            Debug.LogError("Firestore bağlantısı başarısız.");
        }
        else
        {
            Debug.Log("Firestore bağlantısı başarılı.");
        }
    }

    public void CreateUser()
    {
        if (!IsTextFieldsFilled()) return;

        string email = mailField.text;
        string password = passwordField.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            user = result.User;

            CreateUserInFirestore(user);
        });

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void SignInUser()
    {
        if (!IsTextFieldsFilled(true)) return;

        string email = mailField.text;
        string password = passwordField.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(async task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
            user = result.User;

            int photoId = await GetUserPhotoId(user.UserId);
            PlayerPrefs.SetInt("profilePhoto", photoId);

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });
    }

    public async Task<int> GetUserPhotoId(string userId)
    {
        DocumentReference docRef = db.Collection("users").Document(userId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists && snapshot.TryGetValue("profilePhoto", out int photoId))
        {
            return photoId;
        }
        else
        {
            Debug.LogError("User document does not exist or does not contain 'photoId' field.");
            return -1;
        }
    }

    private bool IsTextFieldsFilled(bool isLogin = false)
    {
        if (usernameField.text == "" && !isLogin)
        {
            Debug.LogWarning("Username field is empty.");
            return false;
        }
        if (mailField.text == "" || !IsMailFormatOk())
        {
            Debug.LogWarning("Mail field is empty.");
            return false;
        }
        if (passwordField.text == "" || !IsPasswordOk())
        {
            Debug.LogWarning("Password field is empty.");
            return false;
        }

        return true;
    }

    private bool IsPasswordOk()
    {
        return passwordField.text.Length >= 6;
    }

    private bool IsMailFormatOk()
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(mailField.text);
            return addr.Address == mailField.text;
        }
        catch
        {
            return false;
        }
    
    }

    public void SignOutUser()
    {
        auth.SignOut();
        user = null;
    }

    public void DeleteUser()
    {
        if (user != null)
        {
            user.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }
        else
        {
            Debug.LogWarning("No user is signed in to delete.");
        }
    }

    private void CreateUserInFirestore(FirebaseUser user)
    {

        Debug.Log("Creating user in Firestore.");

        DocumentReference docRef = db.Collection("users").Document(user.UserId);

        var data = new Dictionary<string, object>
        {
            { "email", mailField.text },
            { "username", usernameField.text },
            { "leaderboardRank", 0 },
            { "profilePhoto", 0 },
            { "boughtAvatars", new List<int>() },
            { "points", 0 },
            { "bombJokerCount", 1 },
            { "doubleAnswerJokerCount", 1 },
            { "skipQuestionJokerCount", 1 }
        };

        docRef.SetAsync(data).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error adding document: " + task.Exception);
                foreach (var ex in task.Exception.InnerExceptions)
                {
                    Debug.LogError("Exception: " + ex.Message);
                }
                return;
            }
            Debug.Log("User added to Firestore.");
        });

        PlayerPrefs.SetInt("profilePhoto", 0);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    

    public bool IsUserSignedIn()
    {
        return user != null;
    }

    public FirebaseUser GetUser()
    {
        return user;
    }
}
