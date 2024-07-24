using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [SerializeField] int CORRECT_SCORE = 2, WRONG_SCORE = 1, QUESTION_COUNT = 3;

    public TMP_Text questionText, categoryText;
    public Button answerButton1, answerButton2, answerButton3, answerButton4;

    private FirebaseFirestore db;
    private List<Question> questions = new List<Question>();
    private int currentQuestionIndex = 0;
    private bool isDoubleAnswerActive = false;
    private List<string> possibleAnswers = new List<string>();

    [SerializeField] GameObject loadingPanel;

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
        DisplayNextQuestion();
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
                    questionSnapshot.TryGetValue("options", out string options) &&
                    questionSnapshot.TryGetValue("correctAnswer", out string correct))
                {
                    string[] optionsArray = options.Split(',');
                    List<string> optionsList = new List<string>(optionsArray);
                    optionsList.Shuffle();

                    questions.Add(new Question
                    {
                        Description = question,
                        CorrectAnswer = correct,
                        Options = optionsList
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

    void DisplayNextQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
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
            Debug.Log("No more questions.");
        }
    }

    void CheckAnswer(string selectedAnswer)
    {
        var currentQuestion = questions[currentQuestionIndex - 1];

        if (selectedAnswer == currentQuestion.CorrectAnswer)
        {
            ScoreManager.instance.AddQuizScore(CORRECT_SCORE);
            DisplayNextQuestion();
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
                    }
                }
                isDoubleAnswerActive = false;
                return;
            }
            else
            {
                ScoreManager.instance.RemoveQuizScore(WRONG_SCORE);
                DisplayNextQuestion();
            }
        }
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
}
