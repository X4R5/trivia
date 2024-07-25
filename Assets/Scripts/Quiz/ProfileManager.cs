using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    string username, points;
    int photoId, joker1count, joker2count, joker3count;

    [SerializeField] TMP_Text userNameText, pointsText, joker1countText, joker2countText, joker3countText;
    [SerializeField] Image profileImage;

    [SerializeField] GameObject loadingPanel;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }

    void Start()
    {
        SetAllVariables();
    }

    private void PlaceAllVariables()
    {
        loadingPanel.SetActive(false);

        userNameText.text = "@" + username;
        pointsText.text = points + " Points";
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
            this.photoId = photoId;

        }
        else
        {
            Debug.LogError("User document does not exist or does not contain 'username' field.");
        }
    }

}
