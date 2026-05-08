using System.Collections.Generic;
using UnityEngine;

public class SelectionService : MonoBehaviour
{
    public static SelectionService Instance;

    public PowerUpData selected;
    public List<PowerUpData> deckCards = new List<PowerUpData>();
    public PowerUpData[] equippedSlots = new PowerUpData[6];

    private const int SLOT_COUNT = 6;

    private void Awake()
    {
        Instance = this;
        EnsureSlotArraySize();
    }

    private void EnsureSlotArraySize()
    {
        if (equippedSlots != null && equippedSlots.Length == SLOT_COUNT)
            return;

        PowerUpData[] newSlots = new PowerUpData[SLOT_COUNT];

        if (equippedSlots != null)
        {
            int copyCount = Mathf.Min(equippedSlots.Length, SLOT_COUNT);

            for (int i = 0; i < copyCount; i++)
                newSlots[i] = equippedSlots[i];
        }

        equippedSlots = newSlots;
    }

    public bool HasCard(PowerUpData card)
    {
        return card != null && deckCards.Contains(card);
    }

    public void AddToDeck(PowerUpData card)
    {
        if (card == null) return;

        if (deckCards.Contains(card))
            return;

        deckCards.Add(card);
    }

    public void AddCardAndAutoEquipIfPossible(PowerUpData card)
    {
        if (card == null) return;

        EnsureSlotArraySize();

        AddToDeck(card);

        int emptySlotIndex = GetFirstEmptySlotIndex();

        if (emptySlotIndex != -1)
        {
            equippedSlots[emptySlotIndex] = card;
            Debug.Log("AUTOEQUIP -> " + card.title + " en slot " + emptySlotIndex);
        }
        else
        {
            Debug.Log("DECK -> Slots llenos, carta al deck: " + card.title);
        }
    }

    private int GetFirstEmptySlotIndex()
    {
        EnsureSlotArraySize();

        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (equippedSlots[i] == null)
                return i;
        }

        return -1;
    }

    public void RemoveFromDeck(PowerUpData card)
    {
        if (card == null) return;

        EnsureSlotArraySize();

        deckCards.Remove(card);

        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (equippedSlots[i] == card)
                equippedSlots[i] = null;
        }
    }

    public void EquipToSlot(PowerUpData card, int slotIndex)
    {
        if (card == null) return;

        EnsureSlotArraySize();

        if (slotIndex < 0 || slotIndex >= equippedSlots.Length)
            return;

        equippedSlots[slotIndex] = card;
    }

    public void RemoveFromSlot(int slotIndex)
    {
        EnsureSlotArraySize();

        if (slotIndex < 0 || slotIndex >= equippedSlots.Length)
            return;

        equippedSlots[slotIndex] = null;
    }
}