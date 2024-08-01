using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Firebase.Extensions;
using System.Linq;
using System;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager instance;

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    string username, points;
    int photoId, joker1count, joker2count, joker3count;

    [SerializeField] GameObject profilePopup, spinWheel;
    [SerializeField] Button closeProfilePopupButton;
    [SerializeField] TMP_Text userNameText, pointsText, joker1countText, joker2countText, joker3countText;
    [SerializeField] Image profileImage;
    [SerializeField] List<Sprite> categorySprites = new List<Sprite>();
    [SerializeField] List<GameObject> topCategoryObjects = new List<GameObject>();

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        instance = this;
    }

    void Start()
    {
        SetAllVariables();
        SetBestCategoryVariables();
        closeProfilePopupButton.onClick.AddListener(CloseProfilePopup);
    }

    private void SetBestCategoryVariables()
    {
        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn");

        if (isLoggedIn == 1)
        {
            SetBestCategoryWithFirebase();
        }
        else
        {
            SetBestCategoryWithJsonFile();
        }
    }

    private void SetBestCategoryWithJsonFile()
    {
        var json = Resources.Load<TextAsset>("Answers");
        var answers = JsonUtility.FromJson<Answers>(json.text);

        var allCategories = new List<string> { "Art", "Games", "Geography", "History", "Science", "Sports" };
        var correctCounts = new Dictionary<string, int>();
        var wrongCounts = new Dictionary<string, int>();
        var successPercentages = new Dictionary<string, float>();

        foreach (var category in allCategories)
        {
            var correctCount = (int)answers.GetType().GetField($"{category}Correct").GetValue(answers);
            var wrongCount = (int)answers.GetType().GetField($"{category}Wrong").GetValue(answers);

            correctCounts[category] = correctCount;
            wrongCounts[category] = wrongCount;

            int totalCount = correctCount + wrongCount;
            if (totalCount > 0)
            {
                float percentage = (float)correctCount / totalCount * 100;
                successPercentages[category] = percentage;
            }
            else
            {
                successPercentages[category] = 0;
            }
        }

        var sortedCategories = successPercentages.OrderByDescending(x => x.Value).Take(4).ToList();

        for (int i = 0; i < sortedCategories.Count; i++)
        {
            var category = sortedCategories[i].Key;
            float percentage = sortedCategories[i].Value;

            Transform categoryTransform = topCategoryObjects[i].transform;
            categoryTransform.Find("TrophyImage").gameObject.SetActive(true);
            categoryTransform.Find("CategoryImage").GetComponent<Image>().sprite = categorySprites[allCategories.IndexOf(category)];
            categoryTransform.Find("CategoryImage").Find("CategoryText").GetComponent<TMP_Text>().text = category;
            categoryTransform.Find("PercentText").GetComponent<TMP_Text>().text = $"{percentage:0.##}%";
        }
    }

    private void SetBestCategoryWithFirebase()
    {
        var allCategories = new List<string> { "Art", "Games", "Geography", "History", "Science", "Sports" };
        var correctCounts = new Dictionary<string, int>();
        var wrongCounts = new Dictionary<string, int>();
        var successPercentages = new Dictionary<string, float>();

        var answersDocRef = db.Collection("answers").Document(user.UserId);
        answersDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    foreach (var category in allCategories)
                    {
                        snapshot.TryGetValue($"{category}Correct", out int correctCount);
                        snapshot.TryGetValue($"{category}Wrong", out int wrongCount);

                        correctCounts[category] = correctCount;
                        wrongCounts[category] = wrongCount;

                        int totalCount = correctCount + wrongCount;
                        if (totalCount > 0)
                        {
                            float percentage = (float)correctCount / totalCount * 100;
                            successPercentages[category] = percentage;
                        }
                        else
                        {
                            successPercentages[category] = 0;
                        }
                    }

                    var sortedCategories = successPercentages.OrderByDescending(x => x.Value).Take(4).ToList();

                    for (int i = 0; i < sortedCategories.Count; i++)
                    {
                        var category = sortedCategories[i].Key;
                        float percentage = sortedCategories[i].Value;

                        Transform categoryTransform = topCategoryObjects[i].transform;
                        categoryTransform.Find("TrophyImage").gameObject.SetActive(true);
                        categoryTransform.Find("CategoryImage").GetComponent<Image>().sprite = categorySprites[allCategories.IndexOf(category)];
                        categoryTransform.Find("CategoryImage").Find("CategoryText").GetComponent<TMP_Text>().text = category;
                        categoryTransform.Find("PercentText").GetComponent<TMP_Text>().text = $"{percentage:0.##}%";
                    }
                }
            }
        });
    }

    private void PlaceAllVariables()
    {
        //loadingPanel.SetActive(false);

        userNameText.text = "@" + username;
        pointsText.text = "Points: " + points;
        joker1countText.text = joker1count.ToString();
        joker2countText.text = joker2count.ToString();
        joker3countText.text = joker3count.ToString();

        var allAvatars = Resources.LoadAll<Sprite>("AvatarPack");
        profileImage.sprite = allAvatars[photoId];
    }

    private async void SetAllVariables()
    {
        await SetAllVariablesAsync();
        PlaceAllVariables();
    }

    public async Task SetAllVariablesAsync()
    {
        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn");

        if (isLoggedIn == 1)
        {
            await SetUserVariablesWithFirebase();
        }
        else
        {
            SetUserVariablesWithJsonFile();
        }
    }

    private void SetUserVariablesWithJsonFile()
    {
        var json = Resources.Load<TextAsset>("User");
        var user = JsonUtility.FromJson<User>(json.text);

        username = user.username;
        points = user.points.ToString();
        photoId = user.profilePhoto;
        joker1count = user.bombJokerCount;
        joker2count = user.doubleAnswerJokerCount;
        joker3count = user.skipQuestionJokerCount;
    }

    private async Task SetUserVariablesWithFirebase()
    {
        DocumentReference docRef = db.Collection("users").Document(user.UserId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            snapshot.TryGetValue("username", out string userName);
            snapshot.TryGetValue("points", out int points);
            snapshot.TryGetValue("profilePhoto", out int photoId);
            snapshot.TryGetValue("bombJokerCount", out int joker1count);
            snapshot.TryGetValue("doubleAnswerJokerCount", out int joker2count);
            snapshot.TryGetValue("skipQuestionJokerCount", out int joker3count);

            this.joker1count = joker1count;
            this.joker2count = joker2count;
            this.joker3count = joker3count;
            username = userName;
            this.points = points.ToString();
            Debug.Log("User: " + username + " Points: " + points + " PhotoId: " + photoId);
            this.photoId = photoId;
        }
        else
        {
            Debug.LogError("User document does not exist or does not contain 'username' field.");
        }
    }

    public void OpenProfilePopup()
    {
        spinWheel.SetActive(false);
        profilePopup.transform.localScale = Vector3.zero;
        profilePopup.SetActive(true);

        profilePopup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
    }

    private void CloseProfilePopup()
    {
        profilePopup.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InExpo).OnComplete(OnCloseProfilePopupComplete);
    }

    void OnCloseProfilePopupComplete()
    {
        profilePopup.SetActive(false);
        OpenSpinWheel();
    }

    void OpenSpinWheel()
    {
        spinWheel.transform.localScale = Vector3.zero;
        spinWheel.SetActive(true);

        spinWheel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
    }

}
