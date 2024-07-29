using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.UI;
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
        closeProfilePopupButton.onClick.AddListener(CloseProfilePopup);
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
