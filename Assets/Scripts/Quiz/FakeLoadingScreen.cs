using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class FakeLoadingScreen : MonoBehaviour
{
    public static FakeLoadingScreen instance;

    public Image progressBar;
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.5f;
    public float stallTime = 0.5f;
    public float speedUpFactor = 2f;

    private float fillAmount = 0f;
    private float speed = 1f;
    private bool isStalled = false;

    [SerializeField] private GameObject loadingScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        fillAmount = 0f;
        progressBar.fillAmount = fillAmount;
        StartCoroutine(LoadProgress());
    }

    IEnumerator LoadProgress()
    {
        while (fillAmount < 1f)
        {
            if (!isStalled)
            {
                speed = Random.Range(minSpeed, maxSpeed);
                fillAmount += speed * Time.deltaTime;
                progressBar.fillAmount = Mathf.Clamp01(fillAmount);

                if (Random.value < 0.1f)
                {
                    isStalled = true;
                    yield return new WaitForSeconds(stallTime);
                    isStalled = false;
                }
            }
            else
            {
                yield return null;
            }

            yield return null;
        }

        OnLoadingComplete();
    }

    public void SpeedUpLoading()
    {
        StopCoroutine(LoadProgress());
        StartCoroutine(FastLoadProgress());
        Debug.Log("Speeding up loading...");
    }

    IEnumerator FastLoadProgress()
    {
        float currentFill = progressBar.fillAmount;
        float targetFill = 1f;

        while (currentFill < targetFill)
        {
            speed = Random.Range(minSpeed * speedUpFactor, maxSpeed * speedUpFactor);
            currentFill += speed * Time.deltaTime;
            progressBar.fillAmount = Mathf.Clamp01(currentFill);
            yield return null;
        }

        OnLoadingComplete();
    }

    void OnLoadingComplete()
    {
        StopAllCoroutines();
        loadingScreen.SetActive(false);

        if(QuizManager.instance != null)
        {
            QuizManager.instance.DisplayNextQuestion();
        }
    }
}
