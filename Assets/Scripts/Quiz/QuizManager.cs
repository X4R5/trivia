using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;
using System;
using Firebase.Auth;
using System.IO;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    FirebaseAuth auth;

    [SerializeField] int CORRECT_SCORE = 2, WRONG_SCORE = 1, QUESTION_COUNT = 3;
    [SerializeField] Sprite wrongAnswerButtonSprite, correctAnswerButtonSprite, buttonSprite, timeUpButtonSprite;
    [SerializeField] TMP_Text questionCounterText;

    public TMP_Text questionText, categoryText;
    public Button answerButton1, answerButton2, answerButton3, answerButton4;
    bool canAnswer = false;
    private FirebaseFirestore db;
    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private bool isDoubleAnswerActive = false;
    private List<string> possibleAnswers = new List<string>();

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        instance = this;
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        answerButton1.onClick.AddListener(() => CheckAnswer(answerButton1.GetComponentInChildren<TMP_Text>().text));
        answerButton2.onClick.AddListener(() => CheckAnswer(answerButton2.GetComponentInChildren<TMP_Text>().text));
        answerButton3.onClick.AddListener(() => CheckAnswer(answerButton3.GetComponentInChildren<TMP_Text>().text));
        answerButton4.onClick.AddListener(() => CheckAnswer(answerButton4.GetComponentInChildren<TMP_Text>().text));

        GetQuestions();
    }

    async void GetQuestions()
    {
        await GetQuestionsAsync();
        FakeLoadingScreen.instance.SpeedUpLoading();
    }

    public async Task GetQuestionsAsync()
    {
        string category = PlayerPrefs.GetString("SelectedCategory");
        categoryText.text = category;

        Query query = db.Collection("questions").WhereEqualTo("category", category);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

        Debug.Log("Number of questions: " + querySnapshot.Count);

        if (querySnapshot.Count != 0)
        {
            List<DocumentSnapshot> documents = new List<DocumentSnapshot>(querySnapshot.Documents);
            List<int> indices = new List<int>();

            for (int i = 0; i < QUESTION_COUNT; i++)
            {
                int index;
                do
                {
                    index = UnityEngine.Random.Range(0, documents.Count);
                } while (indices.Contains(index));
                indices.Add(index);

                DocumentSnapshot questionSnapshot = documents[index];
                if (questionSnapshot.Exists &&
                    questionSnapshot.TryGetValue("description", out string question) &&
                    questionSnapshot.TryGetValue("options", out List<string> options) &&
                    questionSnapshot.TryGetValue("correctAnswer", out string correct))
                {
                    options.Shuffle();

                    questions.Add(new Question
                    {
                        Description = question,
                        CorrectAnswer = correct,
                        Options = options
                    });
                }
                else
                {
                    Debug.LogError("Question data is incomplete.");
                }
            }
        }
        else
        {
            Debug.LogError("No documents found for the selected category.");
        }
    }

    public void DisplayNextQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            canAnswer = true;
            isDoubleAnswerActive = false;
            questionCounterText.text = (currentQuestionIndex + 1) + "/" + questions.Count;
            ResetAllButtons();
            QuestionTimeManager.instance.ResetTimer();

            answerButton1.interactable = answerButton2.interactable = answerButton3.interactable = answerButton4.interactable = true;

            var currentQuestion = questions[currentQuestionIndex];
            questionText.text = currentQuestion.Description;
            categoryText.text = PlayerPrefs.GetString("SelectedCategory");

            possibleAnswers = new List<string>(currentQuestion.Options);

            answerButton1.GetComponentInChildren<TMP_Text>().text = possibleAnswers[0];
            answerButton2.GetComponentInChildren<TMP_Text>().text = possibleAnswers[1];
            answerButton3.GetComponentInChildren<TMP_Text>().text = possibleAnswers[2];
            answerButton4.GetComponentInChildren<TMP_Text>().text = possibleAnswers[3];

            currentQuestionIndex++;
        }
        else
        {
            FinishQuiz();
        }
    }

    private void ResetAllButtons()
    {
        answerButton1.image.sprite = answerButton2.image.sprite = answerButton3.image.sprite = answerButton4.image.sprite = buttonSprite;
    }

    private void FinishQuiz()
    {
        PlayerPrefs.SetInt("QuizScore", ScoreManager.instance.GetQuizScore());

        Debug.Log("Quiz finished.");
        Debug.Log("Score: " + ScoreManager.instance.GetQuizScore());
        Debug.Log("Runner scene is loading in 2 seconds...");

        Invoke("LoadRunnerScene", 2f);
    }

    private void LoadRunnerScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Runner");
    }

    void CheckAnswer(string selectedAnswer)
    {
        if (!canAnswer) return;

        var isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0) == 1;

        canAnswer = false;
        var currentQuestion = questions[currentQuestionIndex - 1];

        if (selectedAnswer == currentQuestion.CorrectAnswer)
        {
            ScoreManager.instance.AddQuizScore(CORRECT_SCORE);

            if (isLoggedIn)
            {
                var answersDocRef = db.Collection("answers").Document(auth.CurrentUser.UserId);
                answersDocRef.UpdateAsync($"{PlayerPrefs.GetString("SelectedCategory")}Correct", FieldValue.Increment(1));
            }
            else
            {
                UpdateLocalJsonAnswers($"{PlayerPrefs.GetString("SelectedCategory")}Correct");
            }

            foreach (Button button in new List<Button> { answerButton1, answerButton2, answerButton3, answerButton4 })
            {
                if (button.GetComponentInChildren<TMP_Text>().text == selectedAnswer)
                {
                    button.image.sprite = correctAnswerButtonSprite;
                }
            }
        }
        else
        {
            if (isDoubleAnswerActive)
            {
                foreach (Button button in new List<Button> { answerButton1, answerButton2, answerButton3, answerButton4 })
                {
                    if (button.GetComponentInChildren<TMP_Text>().text == selectedAnswer)
                    {
                        button.interactable = false;
                        button.image.sprite = wrongAnswerButtonSprite;
                        canAnswer = true;
                    }
                }
                isDoubleAnswerActive = false;
                return;
            }
            else
            {
                ScoreManager.instance.RemoveQuizScore(WRONG_SCORE);

                if (isLoggedIn)
                {
                    var answersDocRef = db.Collection("answers").Document(auth.CurrentUser.UserId);
                    answersDocRef.UpdateAsync($"{PlayerPrefs.GetString("SelectedCategory")}Wrong", FieldValue.Increment(1));
                }
                else
                {
                    UpdateLocalJsonAnswers($"{PlayerPrefs.GetString("SelectedCategory")}Wrong");
                }

                foreach (Button button in new List<Button> { answerButton1, answerButton2, answerButton3, answerButton4 })
                {
                    if (button.GetComponentInChildren<TMP_Text>().text == selectedAnswer)
                    {
                        button.image.sprite = wrongAnswerButtonSprite;
                    }
                }
            }
        }

        Invoke("DisplayNextQuestion", 1f);
    }

    void UpdateLocalJsonAnswers(string field)
    {
        var answers = Resources.Load<TextAsset>("Answers");
        var answersData = JsonUtility.FromJson<Answers>(answers.text);

        var currentValue = (int)answersData.GetType().GetField(field).GetValue(answersData);
        answersData.GetType().GetField(field).SetValue(answersData, currentValue + 1);

        var updatedJson = JsonUtility.ToJson(answersData, true); // Pretty print for easier debugging
        var filePath = Path.Combine(Application.dataPath, "Resources/Answers.json");

        File.WriteAllText(filePath, updatedJson);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset("Assets/Resources/Answers.json", UnityEditor.ImportAssetOptions.ForceUpdate);
#endif
    }


    public void SkipCurrentQuestion()
    {
        DisplayNextQuestion();
    }

    public void ApplyDoubleAnswerJoker()
    {
        isDoubleAnswerActive = true;
    }

    public void ApplyBombJoker()
    {
        if (questions.Count > 0 && currentQuestionIndex <= questions.Count)
        {
            var currentQuestion = questions[currentQuestionIndex - 1];
            string correctAnswer = currentQuestion.CorrectAnswer;

            var buttons = new List<Button> { answerButton1, answerButton2, answerButton3, answerButton4 };
            var buttonTexts = new List<string>
            {
                answerButton1.GetComponentInChildren<TMP_Text>().text,
                answerButton2.GetComponentInChildren<TMP_Text>().text,
                answerButton3.GetComponentInChildren<TMP_Text>().text,
                answerButton4.GetComponentInChildren<TMP_Text>().text
            };

            var wrongAnswerIndices = new List<int>();
            for (int i = 0; i < buttonTexts.Count; i++)
            {
                if (buttonTexts[i] != correctAnswer)
                {
                    wrongAnswerIndices.Add(i);
                }
            }

            var randomIndicesToDisable = new HashSet<int>();
            while (randomIndicesToDisable.Count < 2 && wrongAnswerIndices.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, wrongAnswerIndices.Count);
                randomIndicesToDisable.Add(wrongAnswerIndices[randomIndex]);
            }

            foreach (int index in randomIndicesToDisable)
            {
                buttons[index].interactable = false;
            }
        }
    }

    public void TimeUp()
    {
        ScoreManager.instance.RemoveQuizScore(WRONG_SCORE);

        foreach (Button button in new List<Button> { answerButton1, answerButton2, answerButton3, answerButton4 })
        {
            if (button.GetComponentInChildren<TMP_Text>().text == questions[currentQuestionIndex - 1].CorrectAnswer)
            {
                button.image.sprite = timeUpButtonSprite;
            }
        }

        Invoke("DisplayNextQuestion", 1f);
    }
}
