using System.Collections.Generic;
using UnityEngine;

public class SelectionService : MonoBehaviour
{
    public static SelectionService Instance;

    public PowerUpData selected;
    public List<PowerUpData> deckCards = new List<PowerUpData>();
    public PowerUpData[] equippedSlots = new PowerUpData[4];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddToDeck(PowerUpData card)
    {
        if (card == null) return;

        deckCards.Add(card);
        Debug.Log("AddToDeck -> " + card.title + " | Total: " + deckCards.Count);
    }

    public void EquipToSlot(PowerUpData card, int slotIndex)
    {
        if (card == null) return;
        if (slotIndex < 0 || slotIndex >= equippedSlots.Length) return;

        equippedSlots[slotIndex] = card;
    }

    public void RemoveFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSlots.Length) return;

        equippedSlots[slotIndex] = null;
    }

    public void ClearSelection()
    {
        selected = null;
    }
}