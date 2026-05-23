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
    private bool isOpening = false;
    private bool isClosing = false;

    private Coroutine currentRoutine;

    public bool IsOpen => isOpen;
    public bool IsOpenOrOpening => isOpen || isOpening || isClosing;

    private void Awake()
    {
        Instance = this;

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseDeck);
            closeButton.onClick.AddListener(CloseDeck);
        }

        ForceClosed();
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            if (IsLevelUpBusy())
                return;

            if (isOpen)
                CloseDeck();
            else
                OpenDeck();
        }
    }

    public void OpenDeck()
    {
        if (isOpen || isOpening || isClosing)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(OpenRoutine());
    }

    public void CloseDeck()
    {
        if (!isOpen || isClosing)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(CloseRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isOpening = true;

        if (deckPanel == null)
        {
            Debug.LogError("GameplayDeckMenu -> deckPanel no asignado");
            RecoverFromFailedOpen();
            yield break;
        }

        if (deploymentLoader == null)
        {
            Debug.LogError("GameplayDeckMenu -> deploymentLoader no asignado");
            RecoverFromFailedOpen();
            yield break;
        }

        deckPanel.SetActive(true);

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

        yield return null;

        try
        {
            deploymentLoader.Init();
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameplayDeckMenu -> Error cargando el deck:\n" + e);
            RecoverFromFailedOpen();
            yield break;
        }

        Canvas.ForceUpdateCanvases();

        Time.timeScale = 0f;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        float duration = Mathf.Max(0.01f, animationDuration);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
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
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (panelRect != null)
        {
            panelRect.anchoredPosition = visiblePosition;
            panelRect.localScale = Vector3.one;
        }

        isOpen = true;
        isOpening = false;
        currentRoutine = null;
    }

    private IEnumerator CloseRoutine()
    {
        isClosing = true;
        isOpen = false;

        try
        {
            if (ActivationService.Instance != null)
                ActivationService.Instance.RecalculateEquippedPowerUps();
        }
        catch (System.Exception e)
        {
            Debug.LogError("GameplayDeckMenu -> Error recalculando cartas al cerrar deck:\n" + e);
        }

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        Vector2 startPos = panelRect != null ? panelRect.anchoredPosition : visiblePosition;

        float duration = Mathf.Max(0.01f, animationDuration);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
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

        isClosing = false;
        currentRoutine = null;

        LevelUpUI levelUpUI = FindObjectOfType<LevelUpUI>();
        if (levelUpUI != null)
            levelUpUI.TryShowQueuedLevelUp();
    }

    private void RecoverFromFailedOpen()
    {
        ForceClosed();

        Time.timeScale = 1f;

        isOpen = false;
        isOpening = false;
        isClosing = false;
        currentRoutine = null;
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

    private bool IsLevelUpBusy()
    {
        LevelUpUI levelUpUI = FindObjectOfType<LevelUpUI>();

        if (levelUpUI == null)
            return false;

        return levelUpUI.IsShowing || levelUpUI.IsResolvingSelection;
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