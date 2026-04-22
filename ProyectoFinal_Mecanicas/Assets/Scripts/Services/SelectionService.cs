using System.Collections.Generic;
using UnityEngine;

public class SelectionService : MonoBehaviour
{
    public static SelectionService Instance;

    public PowerUpData selected;
    public List<PowerUpData> deckCards = new List<PowerUpData>();

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
        if (card == null)
        {
            Debug.LogError("AddToDeck -> card es null");
            return;
        }

        deckCards.Add(card);
        Debug.Log("Carta añadida al deck: " + card.title + " | Total: " + deckCards.Count);
    }

    public void ClearSelection()
    {
        selected = null;
    }
}