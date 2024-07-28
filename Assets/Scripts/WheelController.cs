using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WheelController : MonoBehaviour
{
    [SerializeField] Transform wheelTransform;
    [SerializeField] float spinDuration = 3.0f, spinSpeed = 720f;
    [SerializeField] TMP_Text selectedCategoryText;
    [SerializeField] Button spinButton;
    [SerializeField] List<string> categories = new List<string> { "Kategori 1", "Kategori 2", "Kategori 3", "Kategori 4", "Kategori 5", "Kategori 6", "Kategori 7" };

    private bool isSpinning = false;

    private void Start()
    {
        spinButton.onClick.AddListener(SpinWheel);
    }

    private void SpinWheel()
    {
        if (!isSpinning)
        {
            StartCoroutine(Spin());
        }
    }

    private IEnumerator Spin()
    {
        isSpinning = true;
        spinButton.interactable = false;
        selectedCategoryText.text = "";

        float targetAngle = Random.Range(0f, 360f) + spinSpeed * spinDuration;
        wheelTransform.DORotate(new Vector3(0f, 0f, targetAngle), spinDuration, RotateMode.FastBeyond360)
                      .SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(spinDuration);

        float finalAngle = wheelTransform.eulerAngles.z % 360;

        string selectedCategory = DetermineCategory(finalAngle);
        selectedCategoryText.text = selectedCategory;
        Debug.Log("Selected Category: " + selectedCategory);

        PlayerPrefs.SetString("SelectedCategory", selectedCategory);

        StartCoroutine(LoadQuizSceneAfterDelay());

        //isSpinning = false;
        //spinButton.interactable = true;
    }

    private IEnumerator LoadQuizSceneAfterDelay()
    {
        Debug.Log("Loading Quiz Scene...");
        yield return new WaitForSeconds(1.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuizPage");
    }

    private string DetermineCategory(float angle)
    {
        int index = Mathf.FloorToInt(angle / (360f / categories.Count));
        return categories[index];
    }
}
