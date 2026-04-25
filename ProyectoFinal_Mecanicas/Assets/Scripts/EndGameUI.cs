using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    public static EndGameUI Instance;

    [Header("Panel")]
    public GameObject panel;
    public CanvasGroup panelCanvasGroup;
    public RectTransform panelRect;

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;

    [Header("Buttons")]
    public Button replayButton;
    public Button mainMenuButton;

    [Header("Animation")]
    public float fadeDuration = 0.5f;
    public float popDuration = 0.35f;
    public float buttonDelay = 0.25f;

    [Header("Scenes")]
    public string gameplaySceneName = "GameScene";
    public string mainMenuSceneName = "MainMenu";

    private bool alreadyShown = false;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(true);

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        if (replayButton != null)
            replayButton.onClick.AddListener(Replay);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(BackToMainMenu);
    }

    public void ShowWin()
    {
        ShowEndScreen("YOU WIN!", "You survived until the end.");
    }

    public void ShowLose()
    {
        ShowEndScreen("GAME OVER", "You were defeated.");
    }

    private void ShowEndScreen(string title, string subtitle)
    {
        if (alreadyShown) return;
        alreadyShown = true;

        Time.timeScale = 0f;

        if (titleText != null)
            titleText.text = title;

        if (subtitleText != null)
            subtitleText.text = subtitle;

        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (panel != null)
            panel.SetActive(true);

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        if (panelRect != null)
            panelRect.localScale = Vector3.one * 0.75f;

        SetButtonVisible(replayButton, false);
        SetButtonVisible(mainMenuButton, false);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;

            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / popDuration;

            float eased = EaseOutBack(t);

            if (panelRect != null)
                panelRect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.75f, Vector3.one, eased);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(buttonDelay);
        yield return StartCoroutine(ShowButtonRoutine(replayButton));

        yield return new WaitForSecondsRealtime(buttonDelay);
        yield return StartCoroutine(ShowButtonRoutine(mainMenuButton));
    }

    private IEnumerator ShowButtonRoutine(Button button)
    {
        if (button == null) yield break;

        GameObject obj = button.gameObject;
        obj.SetActive(true);

        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        RectTransform rt = obj.GetComponent<RectTransform>();

        cg.alpha = 0f;

        if (rt != null)
            rt.localScale = Vector3.one * 0.8f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / 0.25f;

            cg.alpha = Mathf.Lerp(0f, 1f, t);

            if (rt != null)
                rt.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, EaseOutBack(t));

            yield return null;
        }
    }

    private void SetButtonVisible(Button button, bool visible)
    {
        if (button == null) return;

        button.gameObject.SetActive(visible);
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
