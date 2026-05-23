using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeploymentLoader : MonoBehaviour
{
    public Transform deckParent;
    public GameObject cardPrefab;
    public CardSlot[] slots;

    public void Init()
    {
        LoadDeckAndSlots();
    }

    private void LoadDeckAndSlots()
    {
        if (SelectionService.Instance == null)
        {
            Debug.LogError("DeploymentLoader -> SelectionService.Instance es null");
            return;
        }

        if (deckParent == null)
        {
            Debug.LogError("DeploymentLoader -> deckParent no asignado");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("DeploymentLoader -> cardPrefab no asignado");
            return;
        }

        if (slots == null)
        {
            Debug.LogError("DeploymentLoader -> slots no asignado");
            return;
        }

        ClearParent(deckParent);

        foreach (CardSlot slot in slots)
        {
            if (slot == null)
                continue;

            slot.occupied = false;
            slot.currentCard = null;
            ClearParent(slot.transform);
        }

        PowerUpData[] equippedSlots = SelectionService.Instance.equippedSlots;

        if (equippedSlots == null)
        {
            Debug.LogError("DeploymentLoader -> equippedSlots es null");
            return;
        }

        int usableSlots = Mathf.Min(slots.Length, equippedSlots.Length);

        if (slots.Length != equippedSlots.Length)
        {
            Debug.LogWarning(
                "DeploymentLoader -> Slots visuales y slots reales no coinciden. " +
                "Visuales: " + slots.Length + " | Reales: " + equippedSlots.Length
            );
        }

        for (int i = 0; i < usableSlots; i++)
        {
            if (slots[i] == null)
                continue;

            PowerUpData equippedCard = equippedSlots[i];

            if (equippedCard != null)
                CreateCardInParent(equippedCard, slots[i].transform, slots[i]);
        }

        Dictionary<PowerUpData, int> totalCounts = new Dictionary<PowerUpData, int>();

        foreach (PowerUpData card in SelectionService.Instance.deckCards)
        {
            if (card == null)
                continue;

            if (!totalCounts.ContainsKey(card))
                totalCounts[card] = 0;

            totalCounts[card]++;
        }

        Dictionary<PowerUpData, int> equippedCounts = new Dictionary<PowerUpData, int>();

        foreach (PowerUpData card in equippedSlots)
        {
            if (card == null)
                continue;

            if (!equippedCounts.ContainsKey(card))
                equippedCounts[card] = 0;

            equippedCounts[card]++;
        }

        foreach (KeyValuePair<PowerUpData, int> kvp in totalCounts)
        {
            PowerUpData cardData = kvp.Key;
            int total = kvp.Value;
            int equipped = equippedCounts.ContainsKey(cardData) ? equippedCounts[cardData] : 0;

            int copiesToShowInDeck = Mathf.Max(0, total - equipped);

            for (int i = 0; i < copiesToShowInDeck; i++)
                CreateCardInParent(cardData, deckParent, null);
        }
    }

    private void ClearParent(Transform parent)
    {
        if (parent == null)
            return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    private void CreateCardInParent(PowerUpData data, Transform parent, CardSlot targetSlot)
    {
        if (data == null || parent == null || cardPrefab == null)
            return;

        GameObject card = Instantiate(cardPrefab, parent);

        DragCard drag = card.GetComponent<DragCard>();
        if (drag == null)
            drag = card.GetComponentInChildren<DragCard>(true);

        if (drag != null)
        {
            drag.data = data;
            drag.deckParent = deckParent;
            drag.canDrag = true;
        }

        Button[] buttons = card.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
            button.enabled = false;
        }

        EventTrigger[] triggers = card.GetComponentsInChildren<EventTrigger>(true);
        foreach (EventTrigger trigger in triggers)
            trigger.enabled = false;

        LevelUpCardUI levelUpUI = card.GetComponent<LevelUpCardUI>();
        if (levelUpUI == null)
            levelUpUI = card.GetComponentInChildren<LevelUpCardUI>(true);

        if (levelUpUI != null)
            levelUpUI.Setup(data);

        DeckCardUI deckUI = card.GetComponent<DeckCardUI>();
        if (deckUI == null)
            deckUI = card.GetComponentInChildren<DeckCardUI>(true);

        if (deckUI != null)
            deckUI.Setup(data);

        PowerUpCardUI powerUpUI = card.GetComponent<PowerUpCardUI>();
        if (powerUpUI == null)
            powerUpUI = card.GetComponentInChildren<PowerUpCardUI>(true);

        if (powerUpUI != null)
            powerUpUI.Setup(data);

        LayoutElement layoutElement = card.GetComponent<LayoutElement>();

        if (targetSlot != null)
        {
            targetSlot.occupied = true;
            targetSlot.currentCard = drag;

            if (drag != null)
                drag.currentSlot = targetSlot;

            if (layoutElement != null)
                layoutElement.ignoreLayout = true;

            RectTransform rect = card.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;
            }
        }
        else
        {
            if (layoutElement != null)
                layoutElement.ignoreLayout = false;
        }
    }
}