using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;
using Fusion;
using System;

public class FusionQuizManager : NetworkBehaviour, ISpawned
{
    public TMP_Text questionText, categoryText;
    public Button answerButton1, answerButton2, answerButton3, answerButton4;
    string desc, category, option1, option2, option3, option4;

    private FirebaseFirestore db;
    private string correctAnswer;

    [SerializeField] GameObject loadingPanel;
    private FusionScoreManager scoreManager;
    FusionConnection fusionConnection;


    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        scoreManager = FindObjectOfType<FusionScoreManager>();

        UpdateButtonListeners();

        //if (runner.IsServer && runner.IsConnectedToServer)
        //{
        //    Debug.Log("aaaa getting question");
        //    GetQuestion();
        //}
    }

    private void UpdateButtonListeners()
    {
        answerButton1.onClick.AddListener(() => CheckAnswer(answerButton1.GetComponentInChildren<TMP_Text>().text));
        answerButton2.onClick.AddListener(() => CheckAnswer(answerButton2.GetComponentInChildren<TMP_Text>().text));
        answerButton3.onClick.AddListener(() => CheckAnswer(answerButton3.GetComponentInChildren<TMP_Text>().text));
        answerButton4.onClick.AddListener(() => CheckAnswer(answerButton4.GetComponentInChildren<TMP_Text>().text));
    }

    public async void GetQuestion()
    {
        await GetQuestionAsync();
        //loadingPanel.SetActive(false);

        RpcUpdateQuestion(desc, this.category, option1, option2, option3, option4);
    }

    public async Task GetQuestionAsync()
    {
        string category = PlayerPrefs.GetString("SelectedCategory");
        this.category = category;

        Query query = db.Collection("questions").WhereEqualTo("category", category);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

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
                desc = question;
                correctAnswer = correct;

                string[] optionsArray = options.Split(',');
                List<string> optionsList = new List<string>(optionsArray);
                optionsList.Shuffle();

                option1 = optionsList[0]; option2 = optionsList[1]; option3 = optionsList[2]; option4 = optionsList[3];

                //answerButton1.GetComponentInChildren<TMP_Text>().text = optionsList[0];
                //answerButton2.GetComponentInChildren<TMP_Text>().text = optionsList[1];
                //answerButton3.GetComponentInChildren<TMP_Text>().text = optionsList[2];
                //answerButton4.GetComponentInChildren<TMP_Text>().text = optionsList[3];
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

            if (HasStateAuthority)
            {
                int playerIndex = fusionConnection.runner.LocalPlayer.PlayerId;
                scoreManager.UpdateScore(playerIndex, 10);
            }
        }
        else
        {
            Debug.Log("Wrong answer!");
        }

        if (HasStateAuthority)
        {
            GetQuestion();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcUpdateQuestion(string question, string category, string option1, string option2, string option3, string option4)
    {
        questionText.text = question;
        categoryText.text = category;

        answerButton1.GetComponentInChildren<TMP_Text>().text = option1;
        answerButton2.GetComponentInChildren<TMP_Text>().text = option2;
        answerButton3.GetComponentInChildren<TMP_Text>().text = option3;
        answerButton4.GetComponentInChildren<TMP_Text>().text = option4;

        UpdateButtonListeners();
    }
}
