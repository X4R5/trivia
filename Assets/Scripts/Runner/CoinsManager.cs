using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager instance;

    [SerializeField] int coins = 0;
    [SerializeField] TMP_Text coinsText;

    FirebaseAuth auth;
    FirebaseFirestore db;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinsText();
    }

    private void UpdateCoinsText()
    {
        coinsText.text = coins.ToString() + " Coins";
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsText();
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;

        if (coins < 0)
        {
            coins = 0;
        }

        UpdateCoinsText();
    }

    public int GetCoins()
    {
        return coins;
    }

    public async Task OnGameFinished()
    {
        DocumentReference docRef = db.Collection("users").Document(auth.CurrentUser.UserId);
        await docRef.UpdateAsync("points", FieldValue.Increment(coins));
    }

}
