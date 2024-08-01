using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPageController : MonoBehaviour
{
    private void Awake()
    {
        var login = PlayerPrefs.GetInt("login", 0) == 1;
        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;

        if (!login || isLoggedIn)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    public void SignUpScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SignUp");
    }

    public void SignInScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SignIn");
    }
}
