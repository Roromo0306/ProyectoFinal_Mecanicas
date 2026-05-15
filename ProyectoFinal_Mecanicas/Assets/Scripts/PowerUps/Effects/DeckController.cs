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

            var drag = card.GetComponent<DragCard>();
            if (drag != null)
            {
                drag.data = data;
                drag.deckParent = container;
            }

            var powerUpUI = card.GetComponent<PowerUpCardUI>();
            if (powerUpUI != null)
                powerUpUI.Setup(data);

            var deckUI = card.GetComponent<DeckCardUI>();
            if (deckUI != null)
                deckUI.Setup(data);
        }
    }
}