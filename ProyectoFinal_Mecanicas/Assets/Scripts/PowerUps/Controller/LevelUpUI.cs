using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    public GameObject panel;
    public Transform cardContainer;
    public GameObject cardPrefab;
    public List<PowerUpData> allPowerUps;

    private bool isShowing = false;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        ClearCards();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<LevelUpEvent>(OnLevelUp);
    }

    private void OnLevelUp(object evt)
    {
        if (isShowing)
            return;

        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        panel.SetActive(true);
        Time.timeScale = 0;

        ClearCards();

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 800);

        yield return MovePanel();

        List<PowerUpData> pool = new List<PowerUpData>(allPowerUps);
        List<PowerUpData> selection = new();

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            selection.Add(pool[index]);
            pool.RemoveAt(index);
        }

        foreach (var data in selection)
        {
            var card = Instantiate(cardPrefab, cardContainer);

            var ui = card.GetComponentInChildren<LevelUpCardUI>(true);
            if (ui != null)
                ui.Setup(data);

            var cg = card.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }

            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    private void ClearCards()
    {
        if (cardContainer == null) return;

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

        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
        isShowing = false;
    }
}