using System.Collections.Generic;
using UnityEngine;

public class DeploymentLoader : MonoBehaviour
{
    public Transform deckParent;
    public GameObject cardPrefab;
    public CardSlot[] slots;

    public void Init()
    {
        LoadDeckAndSlots();
    }

    void LoadDeckAndSlots()
    {
        if (SelectionService.Instance == null)
        {
            Debug.LogError("SelectionService.Instance es null");
            return;
        }

        foreach (Transform child in deckParent)
            Destroy(child.gameObject);

        foreach (var slot in slots)
            slot.ClearVisualOnly();

        // 1. Pintar cartas equipadas en slots
        for (int i = 0; i < slots.Length; i++)
        {
            PowerUpData equippedCard = SelectionService.Instance.equippedSlots[i];

            if (equippedCard != null)
            {
                CreateCardInParent(equippedCard, slots[i].transform, slots[i]);
            }
        }

        // 2. Contar cuántas copias de cada carta hay en el deck total
        Dictionary<PowerUpData, int> totalCounts = new Dictionary<PowerUpData, int>();
        foreach (var card in SelectionService.Instance.deckCards)
        {
            if (card == null) continue;

            if (!totalCounts.ContainsKey(card))
                totalCounts[card] = 0;

            totalCounts[card]++;
        }

        // 3. Contar cuántas copias de cada carta están equipadas
        Dictionary<PowerUpData, int> equippedCounts = new Dictionary<PowerUpData, int>();
        foreach (var card in SelectionService.Instance.equippedSlots)
        {
            if (card == null) continue;

            if (!equippedCounts.ContainsKey(card))
                equippedCounts[card] = 0;

            equippedCounts[card]++;
        }

        // 4. Pintar en deck solo las copias sobrantes
        foreach (var kvp in totalCounts)
        {
            PowerUpData cardData = kvp.Key;
            int total = kvp.Value;
            int equipped = equippedCounts.ContainsKey(cardData) ? equippedCounts[cardData] : 0;

            int copiesToShowInDeck = total - equipped;

            Debug.Log(cardData.title + " -> total: " + total + " | equipped: " + equipped + " | en deck visual: " + copiesToShowInDeck);

            for (int i = 0; i < copiesToShowInDeck; i++)
            {
                CreateCardInParent(cardData, deckParent, null);
            }
        }
    }

    void CreateCardInParent(PowerUpData data, Transform parent, CardSlot targetSlot)
    {
        GameObject card = Instantiate(cardPrefab, parent);

        DragCard drag = card.GetComponent<DragCard>();
        if (drag != null)
        {
            drag.data = data;
            drag.deckParent = deckParent;
        }

        LevelUpCardUI levelUpUI = card.GetComponent<LevelUpCardUI>();
        if (levelUpUI != null)
            levelUpUI.Setup(data);

        DeckCardUI deckUI = card.GetComponent<DeckCardUI>();
        if (deckUI != null)
            deckUI.Setup(data);

        PowerUpCardUI powerUpUI = card.GetComponent<PowerUpCardUI>();
        if (powerUpUI != null)
            powerUpUI.Setup(data);

        if (targetSlot != null)
        {
            targetSlot.occupied = true;
            targetSlot.currentCard = drag;

            if (drag != null)
                drag.currentSlot = targetSlot;

            RectTransform rect = card.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
            }
        }
    }
}