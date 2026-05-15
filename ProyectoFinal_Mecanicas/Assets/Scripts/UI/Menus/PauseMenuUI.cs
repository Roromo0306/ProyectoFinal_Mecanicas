using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    [Header("Input")]
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    public GameObject pausePanel;
    public CanvasGroup canvasGroup;
    public RectTransform panelRect;

    [Header("Buttons")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Animation")]
    public float animationDuration = 0.25f;
    public float startScale = 0.75f;

    private bool isPaused = false;
    private bool isAnimating = false;
    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(BackToMainMenu);

        ForceClosed();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isAnimating) return;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        SFXManager.Instance?.PlayOpenDeck();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(OpenRoutine());
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        SFXManager.Instance?.PlayCloseDeck();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(CloseRoutine(true));
    }

    public void RestartGame()
    {
        if (isAnimating) return;

        SFXManager.Instance?.PlayButtonClick();

        StartCoroutine(RestartRoutine());
    }

    public void BackToMainMenu()
    {
        if (isAnimating) return;

        SFXManager.Instance?.PlayButtonClick();

        StartCoroutine(MainMenuRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isAnimating = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
        }

        if (panelRect != null)
            panelRect.localScale = Vector3.one * startScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animationDuration;
            float eased = EaseOutBack(t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            if (panelRect != null)
                panelRect.localScale = Vector3.LerpUnclamped(Vector3.one * startScale, Vector3.one, eased);

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (panelRect != null)
            panelRect.localScale = Vector3.one;

        isAnimating = false;
    }

    private IEnumerator CloseRoutine(bool resumeTime)
    {
        isAnimating = true;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        float t = 0f;

        Vector3 start = panelRect != null ? panelRect.localScale : Vector3.one;
        Vector3 end = Vector3.one * startScale;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animationDuration;
            float eased = EaseInBack(t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            if (panelRect != null)
                panelRect.localScale = Vector3.LerpUnclamped(start, end, eased);

            yield return null;
        }

        ForceClosed();

        if (resumeTime)
            Time.timeScale = 1f;

        isAnimating = false;
    }

    private IEnumerator RestartRoutine()
    {
        isAnimating = true;

        Time.timeScale = 1f;

        if (SceneFade.Instance != null)
        {
            SceneFade.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        yield return null;
    }

    private IEnumerator MainMenuRoutine()
    {
        isAnimating = true;

        Time.timeScale = 1f;

        if (SceneFade.Instance != null)
        {
            SceneFade.Instance.LoadSceneWithFade(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        yield return null;
    }

    private void ForceClosed()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (panelRect != null)
            panelRect.localScale = Vector3.one * startScale;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    private float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return c3 * x * x * x - c1 * x * x;
    }
}