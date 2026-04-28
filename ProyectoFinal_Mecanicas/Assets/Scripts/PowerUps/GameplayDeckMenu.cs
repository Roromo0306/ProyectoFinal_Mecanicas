using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameplayDeckMenu : MonoBehaviour
{
    public static GameplayDeckMenu Instance;

    [Header("Input")]
    public KeyCode openKey = KeyCode.Tab;

    [Header("Deck UI")]
    public GameObject deckPanel;
    public CanvasGroup canvasGroup;
    public RectTransform panelRect;
    public DeploymentLoader deploymentLoader;
    public Button closeButton;

    [Header("Animation")]
    public float animationDuration = 0.25f;
    public Vector2 hiddenPosition = new Vector2(0f, -900f);
    public Vector2 visiblePosition = Vector2.zero;

    private bool isOpen = false;
    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDeck);

        ForceClosed();
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            if (isOpen)
                CloseDeck();
            else
                OpenDeck();
        }
    }

    public void OpenDeck()
    {
        if (isOpen) return;

        isOpen = true;
        Time.timeScale = 0f;

        if (deploymentLoader != null)
            deploymentLoader.Init();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(OpenRoutine());
    }

    public void CloseDeck()
    {
        if (!isOpen) return;

        isOpen = false;

        if (ActivationService.Instance != null)
            ActivationService.Instance.RecalculateEquippedPowerUps();

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(CloseRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        deckPanel.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (panelRect != null)
        {
            panelRect.anchoredPosition = hiddenPosition;
            panelRect.localScale = Vector3.one * 0.9f;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animationDuration;
            float eased = EaseOutBack(t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            if (panelRect != null)
            {
                panelRect.anchoredPosition = Vector2.LerpUnclamped(hiddenPosition, visiblePosition, eased);
                panelRect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.9f, Vector3.one, eased);
            }

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (panelRect != null)
        {
            panelRect.anchoredPosition = visiblePosition;
            panelRect.localScale = Vector3.one;
        }
    }

    private IEnumerator CloseRoutine()
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        Vector2 startPos = panelRect != null ? panelRect.anchoredPosition : visiblePosition;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / animationDuration;
            float eased = EaseInBack(t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            if (panelRect != null)
            {
                panelRect.anchoredPosition = Vector2.LerpUnclamped(startPos, hiddenPosition, eased);
                panelRect.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 0.9f, t);
            }

            yield return null;
        }

        ForceClosed();
        Time.timeScale = 1f;
    }

    private void ForceClosed()
    {
        if (deckPanel != null)
            deckPanel.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (panelRect != null)
        {
            panelRect.anchoredPosition = hiddenPosition;
            panelRect.localScale = Vector3.one * 0.9f;
        }
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