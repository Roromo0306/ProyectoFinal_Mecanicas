using UnityEngine;
using UnityEngine.EventSystems;

public class DeckDropZone : MonoBehaviour, IDropHandler
{
    public Transform deckParent;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        DragCard dragCard = draggedObject.GetComponent<DragCard>();
        if (dragCard == null) return;

        if (dragCard.currentSlot != null && SelectionService.Instance != null)
        {
            SelectionService.Instance.RemoveFromSlot(dragCard.currentSlot.slotIndex);
        }

        dragCard.droppedSuccessfully = true;
        dragCard.ReturnToDeck();
    }
}