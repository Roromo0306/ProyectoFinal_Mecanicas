using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform container;

    public void LoadCards(List<PowerUpData> cards)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (var data in cards)
        {
            GameObject card = Instantiate(cardPrefab, container);

            var ui = card.GetComponent<PowerUpDragUI>();
            ui.Data = data;
        }
    }
}