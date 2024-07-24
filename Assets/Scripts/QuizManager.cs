using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;

public class QuizManager : MonoBehaviour
{


    public TMP_Text questionText, categoryText;

    public Button answerButton1;
    public Button answerButton2;
    public Button answerButton3;
    public Button answerButton4;

    private FirebaseFirestore db;
    private string correctAnswer;

    [SerializeField] GameObject loadingPanel;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        answerButton1.onClick.AddListener(() => CheckAnswer(answerButton1.GetComponentInChildren<TMP_Text>().text));
        answerButton2.onClick.AddListener(() => CheckAnswer(answerButton2.GetComponentInChildren<TMP_Text>().text));
        answerButton3.onClick.AddListener(() => CheckAnswer(answerButton3.GetComponentInChildren<TMP_Text>().text));
        answerButton4.onClick.AddListener(() => CheckAnswer(answerButton4.GetComponentInChildren<TMP_Text>().text));

        GetQuestion();
    }

    async void GetQuestion()
    {
        await GetQuestionAsync();
        loadingPanel.SetActive(false);
    }

    public async Task GetQuestionAsync()
    {
        string category = PlayerPrefs.GetString("SelectedCategory");

        categoryText.text = category;

        Query query = db.Collection("questions").WhereEqualTo("category", category);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

        Debug.Log("Number of questions: " + querySnapshot.Count);

        if (querySnapshot.Count != 0)
        {


            int randomIndex = UnityEngine.Random.Range(0, querySnapshot.Count);
            List<DocumentSnapshot> documents = new List<DocumentSnapshot>(querySnapshot.Documents);
            DocumentSnapshot randomQuestionSnapshot = documents[randomIndex];

            if (randomQuestionSnapshot.Exists &&
                randomQuestionSnapshot.TryGetValue("description", out string question) &&
                randomQuestionSnapshot.TryGetValue("options", out string options) &&
                randomQuestionSnapshot.TryGetValue("correctAnswer", out string correct))
            {
                questionText.text = question;
                correctAnswer = correct;

                string[] optionsArray = options.Split(',');
                List<string> optionsList = new List<string>(optionsArray);
                optionsList.Shuffle();

                answerButton1.GetComponentInChildren<TMP_Text>().text = optionsList[0];
                answerButton2.GetComponentInChildren<TMP_Text>().text = optionsList[1];
                answerButton3.GetComponentInChildren<TMP_Text>().text = optionsList[2];
                answerButton4.GetComponentInChildren<TMP_Text>().text = optionsList[3];
            }
            else
            {
                Debug.LogError("Question data is incomplete.");
            }
        }
        else
        {
            Debug.LogError("No documents found for the selected category.");
        }
    }


    void CheckAnswer(string selectedAnswer)
    {
        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("Correct answer!");
        }
        else
        {
            Debug.Log("Wrong answer!");
        }
    }
}
