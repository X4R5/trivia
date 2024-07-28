using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;  // Json.NET kütüphanesini kullanmak için ekleyin
using static UnityEditor.Progress;
using System.Threading.Tasks;

public class QuestionAddManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField jsonInputField;
    [SerializeField] private Button addButton;

    private FirebaseFirestore db;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        addButton.onClick.AddListener(SaveQuestionsToFirestore);
    }

    public async void SaveQuestionsToFirestore()
    {
        string jsonInput = jsonInputField.text;
        Debug.Log("JSON Input: " + jsonInput);

        try
        {
            QuestionList questionList = JsonConvert.DeserializeObject<QuestionList>(jsonInput);
            if (questionList == null || questionList.questions == null)
            {
                Debug.LogError("Failed to parse JSON or questions list is null.");
                return;
            }

            Debug.Log("Questions Count: " + questionList.questions.Count);
            for (int i = 0; i < questionList.questions.Count; i++)
            {
                await SaveQuestion(questionList.questions[i], PlayerPrefs.GetInt("Latest" + questionList.questions[i].Category + "Id") + 1);
                PlayerPrefs.SetInt("Latest" + questionList.questions[i].Category + "Id", PlayerPrefs.GetInt("Latest" + questionList.questions[i].Category + "Id") + 1);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
        }
    }

    public async Task SaveQuestion(Question question, int id)
    {
        string docId = $"{question.Category}{id}";
        DocumentReference docRef = db.Collection("questions").Document(docId);

        var data = new Dictionary<string, object>
        {
            { "correctAnswer", question.CorrectAnswer },
            { "description", question.Description },
            { "options", question.Options },
            { "category", question.Category }
        };

        await docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Error adding document {docId}: {task.Exception}");
            }
            else
            {
                Debug.Log($"Document {docId} added successfully.");
            }
        });
    }
}

[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}
