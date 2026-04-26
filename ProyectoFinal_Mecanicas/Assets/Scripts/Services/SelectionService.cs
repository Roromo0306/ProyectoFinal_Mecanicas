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
        Instance = this;
    }

    public bool HasCard(PowerUpData card)
    {
        if (card == null) return false;
        return deckCards.Contains(card);
    }

    public bool AddToDeck(PowerUpData card)
    {
        if (card == null) return false;

        if (deckCards.Contains(card))
        {
            Debug.Log("Carta ya existe en deck: " + card.title);
            return false;
        }

        deckCards.Add(card);
        Debug.Log("Carta añadida al deck: " + card.title);
        return true;
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
}