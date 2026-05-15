using UnityEngine;
using UnityEngine.EventSystems;

public class RecycleZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        DragCard card = draggedObject.GetComponent<DragCard>();
        if (card == null || card.data == null) return;

        card.droppedSuccessfully = true;

        if (card.currentSlot != null)
        {
            if (SelectionService.Instance != null)
                SelectionService.Instance.RemoveFromSlot(card.currentSlot.slotIndex);

            card.currentSlot.ClearSlot();
            card.currentSlot = null;
        }

        if (SelectionService.Instance != null)
            SelectionService.Instance.RemoveFromDeck(card.data);

        if (ActivationService.Instance != null)
            ActivationService.Instance.RecalculateEquippedPowerUps();
        SFXManager.Instance?.PlayRecycle();

        Destroy(card.gameObject);

        Debug.Log("Carta reciclada: " + card.data.title);
    }
}