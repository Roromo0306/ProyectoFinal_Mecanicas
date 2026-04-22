using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpUI : MonoBehaviour
{
    public GameObject panel;
    public Transform cardContainer;
    public GameObject cardPrefab;
    public List<PowerUpData> allPowerUps;

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
        Debug.Log("EVENTO RECIBIDO EN UI");
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
            var ui = card.GetComponent<LevelUpCardUI>();
            if (ui != null)
                ui.Setup(data);

            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    void ClearCards()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
    }

    IEnumerator MovePanel()
    {
        float t = 0;
        Vector3 start = new Vector3(0, 800, 0);
        Vector3 end = Vector3.zero;

        RectTransform rt = panel.GetComponent<RectTransform>();

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 3;
            rt.anchoredPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }
}