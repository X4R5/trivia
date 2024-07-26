using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;

    private FirebaseFirestore db;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        LoadLeaderboard();
    }

    private void LoadLeaderboard()
    {
        db.Collection("users").OrderByDescending("points").Limit(10).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading leaderboard: " + task.Exception);
                return;
            }

            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                string username = document.GetValue<string>("username");
                int points = document.GetValue<int>("points");

                GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                leaderboardEntry.transform.Find("UsernameText").GetComponent<TMP_Text>().text = "@" + username;
                leaderboardEntry.transform.Find("PointsText").GetComponent<TMP_Text>().text = points.ToString() + " Points";
            }
        });
    }
}
