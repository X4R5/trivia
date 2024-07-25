using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            await CoinsManager.instance.OnGameFinished();

            SceneManager.LoadScene("MainMenu");
        }
    }
}
