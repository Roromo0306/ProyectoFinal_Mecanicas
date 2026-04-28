using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    public string gameplaySceneName = "GameScene";

    [Header("Main UI")]
    public RectTransform titleText;
    public RectTransform buttonsPanel;

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;
    public Button optionsBackButton;
    public Button creditsBackButton;

    [Header("Fade")]
    public SceneFade sceneFade;

    [Header("Juice")]
    public float introDuration = 0.45f;
    public float buttonPopDelay = 0.08f;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        playButton.onClick.AddListener(Play);
        optionsButton.onClick.AddListener(OpenOptions);
        creditsButton.onClick.AddListener(OpenCredits);
        quitButton.onClick.AddListener(Quit);

        if (optionsBackButton != null)
            optionsBackButton.onClick.AddListener(ClosePanels);

        if (creditsBackButton != null)
            creditsBackButton.onClick.AddListener(ClosePanels);
    }

    private void Start()
    {
        StartCoroutine(IntroAnimation());
    }

    private IEnumerator IntroAnimation()
    {
        if (titleText != null)
        {
            titleText.localScale = Vector3.zero;
            StartCoroutine(Pop(titleText, introDuration));
        }

        if (buttonsPanel != null)
        {
            foreach (Transform child in buttonsPanel)
                child.localScale = Vector3.zero;

            yield return new WaitForSecondsRealtime(0.2f);

            foreach (Transform child in buttonsPanel)
            {
                StartCoroutine(Pop(child as RectTransform, introDuration));
                yield return new WaitForSecondsRealtime(buttonPopDelay);
            }
        }
    }

    private IEnumerator Pop(RectTransform target, float duration)
    {
        if (target == null) yield break;

        float t = 0f;
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float eased = EaseOutBack(t);

            target.localScale = Vector3.LerpUnclamped(start, end, eased);

            yield return null;
        }

        target.localScale = Vector3.one;
    }

    public void Play()
    {
        DisableButtons();

        if (SceneFade.Instance != null)
            SceneFade.Instance.LoadSceneWithFade(gameplaySceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameplaySceneName);
    }

    private IEnumerator PlayRoutine()
    {
        DisableButtons();

        if (SceneFade.Instance != null)
        {
            SceneFade.Instance.transform.SetAsLastSibling();
            yield return SceneFade.Instance.FadeOut();
        }

        SceneManager.LoadScene(gameplaySceneName);

        if (SceneFade.Instance != null)
            yield return SceneFade.Instance.FadeIn();
    }

    private void OpenOptions()
    {
        ClosePanels();

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            StartCoroutine(Pop(optionsPanel.GetComponent<RectTransform>(), 0.25f));
        }
    }

    private void OpenCredits()
    {
        ClosePanels();

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            StartCoroutine(Pop(creditsPanel.GetComponent<RectTransform>(), 0.25f));
        }
    }

    private void ClosePanels()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    private void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void DisableButtons()
    {
        playButton.interactable = false;
        optionsButton.interactable = false;
        creditsButton.interactable = false;
        quitButton.interactable = false;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}