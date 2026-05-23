using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public Transform cardContainer;
    public GameObject cardPrefab;
    public List<PowerUpData> allPowerUps;

    [Header("Effects")]
    [SerializeField] private LevelUpUIParticleEffect levelUpParticleEffect;

    [Header("Card Unlock Rules")]
    [SerializeField] private int ownedCardsBeforeIgnoringLevelLocks = 5;

    private bool isShowing = false;
    private bool isResolvingSelection = false;

    private Coroutine showRoutine;
    private Coroutine chooseRoutine;

    private readonly Queue<int> pendingLevelUps = new Queue<int>();

    public bool IsShowing => isShowing;
    public bool IsResolvingSelection => isResolvingSelection;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        ClearCards();

        isShowing = false;
        isResolvingSelection = false;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<LevelUpEvent>(OnLevelUp);
    }

    private void OnLevelUp(LevelUpEvent levelUpEvent)
    {
        int level = Mathf.Max(1, levelUpEvent.newLevel);

        if (isShowing || isResolvingSelection || IsDeckBusy())
        {
            pendingLevelUps.Enqueue(level);
            return;
        }

        SFXManager.Instance?.PlayLevelUp();
        StartShowing(level);
    }

    private void StartShowing(int playerLevel)
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine(playerLevel));
    }

    private IEnumerator ShowRoutine(int playerLevel)
    {
        isShowing = true;

        if (panel != null)
            panel.SetActive(true);

        if (levelUpParticleEffect != null)
            levelUpParticleEffect.Play();

        Time.timeScale = 0f;

        ClearCards();

        RectTransform rt = panel != null ? panel.GetComponent<RectTransform>() : null;

        if (rt != null)
            rt.anchoredPosition = new Vector2(0f, 800f);

        yield return MovePanel();

        if (!isShowing)
            yield break;

        List<PowerUpData> pool = BuildAvailablePowerUpPool(playerLevel);

        if (pool.Count <= 0)
        {
            Debug.Log("LevelUpUI -> No quedan cartas nuevas disponibles.");
            ClosePanel(true);
            TryShowQueuedLevelUp();
            yield break;
        }

        List<PowerUpData> selection = GetRandomSelection(pool, 3);

        foreach (PowerUpData data in selection)
        {
            if (!isShowing)
                yield break;

            GameObject card = Instantiate(cardPrefab, cardContainer);

            LevelUpCardUI ui = card.GetComponentInChildren<LevelUpCardUI>(true);
            if (ui != null)
                ui.Setup(data);

            DragCard drag = card.GetComponentInChildren<DragCard>(true);
            if (drag != null)
                drag.canDrag = false;

            CanvasGroup cg = card.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }

            yield return new WaitForSecondsRealtime(0.2f);
        }

        showRoutine = null;
    }

    public void ChooseCard(PowerUpData data)
    {
        if (isResolvingSelection)
            return;

        if (data == null)
        {
            Debug.LogError("LevelUpUI.ChooseCard -> data es null");
            return;
        }

        if (chooseRoutine != null)
            StopCoroutine(chooseRoutine);

        chooseRoutine = StartCoroutine(ChooseCardRoutine(data));
    }

    private IEnumerator ChooseCardRoutine(PowerUpData data)
    {
        isResolvingSelection = true;

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        SFXManager.Instance?.PlayCardSelect();

        if (SelectionService.Instance == null)
        {
            Debug.LogError("LevelUpUI -> SelectionService.Instance es null");
            ClosePanel(true);
            isResolvingSelection = false;
            chooseRoutine = null;
            yield break;
        }

        SelectionService.Instance.selected = data;

        // Si hay hueco, se equipa.
        // Si no hay hueco, se queda en el deck.
        SelectionService.Instance.AddCardAndAutoEquipIfPossible(data);

        if (ActivationService.Instance != null)
            ActivationService.Instance.RecalculateEquippedPowerUps();

        // Cerramos el level up pero NO reanudamos todavía,
        // porque ahora tiene que abrirse el deck.
        ClosePanel(false);

        // Esperamos un par de frames limpios para evitar conflictos con Destroy(),
        // pooling de enemigos y reconstrucción del DeckPanel.
        yield return null;
        yield return null;

        if (GameplayDeckMenu.Instance != null)
        {
            GameplayDeckMenu.Instance.OpenDeck();
        }
        else if (UIFlowController.Instance != null)
        {
            UIFlowController.Instance.OpenDeployment();
        }
        else
        {
            Debug.LogError("LevelUpUI -> No existe GameplayDeckMenu ni UIFlowController.");
            Time.timeScale = 1f;
        }

        isResolvingSelection = false;
        chooseRoutine = null;
    }

    private List<PowerUpData> BuildAvailablePowerUpPool(int playerLevel)
    {
        List<PowerUpData> pool = new List<PowerUpData>();
        bool ignoreLevelLocks = ShouldIgnoreLevelLocks();

        foreach (PowerUpData powerUp in allPowerUps)
        {
            if (powerUp == null)
                continue;

            if (SelectionService.Instance != null && SelectionService.Instance.HasCard(powerUp))
                continue;

            if (!ignoreLevelLocks && playerLevel < powerUp.unlockLevel)
                continue;

            pool.Add(powerUp);
        }

        return pool;
    }

    private bool ShouldIgnoreLevelLocks()
    {
        if (SelectionService.Instance == null)
            return false;

        return SelectionService.Instance.deckCards.Count >= ownedCardsBeforeIgnoringLevelLocks;
    }

    private List<PowerUpData> GetRandomSelection(List<PowerUpData> pool, int maxCards)
    {
        List<PowerUpData> selection = new List<PowerUpData>();

        for (int i = 0; i < maxCards && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            selection.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return selection;
    }

    private void ClearCards()
    {
        if (cardContainer == null)
            return;

        for (int i = cardContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = cardContainer.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    private IEnumerator MovePanel()
    {
        if (panel == null)
            yield break;

        float t = 0f;
        Vector3 start = new Vector3(0f, 800f, 0f);
        Vector3 end = Vector3.zero;

        RectTransform rt = panel.GetComponent<RectTransform>();

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 3f;

            if (rt != null)
                rt.anchoredPosition = Vector3.Lerp(start, end, t);

            yield return null;
        }
    }

    public void Hide()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        ClosePanel(true);
        TryShowQueuedLevelUp();
    }

    private void ClosePanel(bool resumeGame)
    {
        ClearCards();

        if (levelUpParticleEffect != null)
            levelUpParticleEffect.Stop();

        if (panel != null)
            panel.SetActive(false);

        if (resumeGame)
            Time.timeScale = 1f;

        isShowing = false;
    }

    public void TryShowQueuedLevelUp()
    {
        if (pendingLevelUps.Count <= 0)
            return;

        if (isShowing || isResolvingSelection || IsDeckBusy())
            return;

        int nextLevel = pendingLevelUps.Dequeue();
        SFXManager.Instance?.PlayLevelUp();
        StartShowing(nextLevel);
    }

    private bool IsDeckBusy()
    {
        return GameplayDeckMenu.Instance != null && GameplayDeckMenu.Instance.IsOpenOrOpening;
    }

    public void MarkClosed()
    {
        isShowing = false;
    }
}