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

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        ClearCards();
        isShowing = false;
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
        if (isShowing)
            return;

        SFXManager.Instance?.PlayLevelUp();

        StartCoroutine(ShowRoutine(levelUpEvent.newLevel));
    }

    IEnumerator ShowRoutine(int playerLevel)
    {
        isShowing = true;

        if (panel != null)
            panel.SetActive(true);

        if (levelUpParticleEffect != null)
            levelUpParticleEffect.Play();

        Time.timeScale = 0f;

        ClearCards();

        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = new Vector2(0, 800);

        yield return MovePanel();

        List<PowerUpData> pool = BuildAvailablePowerUpPool(playerLevel);

        if (pool.Count <= 0)
        {
            Debug.Log("LevelUpUI -> No quedan cartas nuevas disponibles.");
            Hide();
            yield break;
        }

        List<PowerUpData> selection = GetRandomSelection(pool, 3);

        foreach (PowerUpData data in selection)
        {
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

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
    }

    private IEnumerator MovePanel()
    {
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
        ClearCards();

        if (levelUpParticleEffect != null)
            levelUpParticleEffect.Stop();

        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
        isShowing = false;
    }

    public void MarkClosed()
    {
        isShowing = false;
    }
}