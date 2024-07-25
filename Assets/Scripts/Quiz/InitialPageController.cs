using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPageController : MonoBehaviour
{
    public void SignUpScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SignUp");
    }

    public void SignInScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SignIn");
    }
}
