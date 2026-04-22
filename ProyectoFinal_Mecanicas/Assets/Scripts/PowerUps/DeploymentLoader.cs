using UnityEngine;

public class DeploymentLoader : MonoBehaviour
{
    public Transform deckParent;
    public GameObject cardPrefab;

    public void Init()
    {
        LoadDeck();
    }

    void LoadDeck()
    {
        if (SelectionService.Instance == null)
        {
            Debug.LogError("SelectionService.Instance es null");
            return;
        }

        foreach (Transform child in deckParent)
            Destroy(child.gameObject);

        var deckCards = SelectionService.Instance.deckCards;

        if (deckCards == null || deckCards.Count == 0)
        {
            Debug.LogWarning("No hay cartas en el deck");
            return;
        }

        foreach (var data in deckCards)
        {
            var card = Instantiate(cardPrefab, deckParent);

            var drag = card.GetComponent<DragCard>();
            if (drag != null)
            {
                drag.data = data;
                drag.deckParent = deckParent;
            }

            var levelUpUI = card.GetComponent<LevelUpCardUI>();
            if (levelUpUI != null)
                levelUpUI.Setup(data);

            var deckUI = card.GetComponent<DeckCardUI>();
            if (deckUI != null)
                deckUI.Setup(data);

            var powerUpUI = card.GetComponent<PowerUpCardUI>();
            if (powerUpUI != null)
                powerUpUI.Setup(data);
        }

        Debug.Log("Deck cargado con " + deckCards.Count + " cartas");
    }
}