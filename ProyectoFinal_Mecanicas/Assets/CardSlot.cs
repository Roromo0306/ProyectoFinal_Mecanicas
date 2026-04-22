using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public bool occupied = false;
    public DragCard currentCard;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        DragCard dragCard = draggedObject.GetComponent<DragCard>();
        if (dragCard == null || dragCard.data == null) return;

        if (occupied && currentCard != null && currentCard != dragCard)
            return;

        dragCard.droppedSuccessfully = true;

        draggedObject.transform.SetParent(transform, false);

        LayoutElement le = draggedObject.GetComponent<LayoutElement>();
        if (le != null)
            le.ignoreLayout = true;

        RectTransform draggedRect = draggedObject.GetComponent<RectTransform>();
        if (draggedRect != null)
        {
            draggedRect.anchorMin = new Vector2(0.5f, 0.5f);
            draggedRect.anchorMax = new Vector2(0.5f, 0.5f);
            draggedRect.pivot = new Vector2(0.5f, 0.5f);
            draggedRect.anchoredPosition = Vector2.zero;
            draggedRect.localScale = Vector3.one;
            draggedRect.localRotation = Quaternion.identity;
        }

        occupied = true;
        currentCard = dragCard;
        dragCard.currentSlot = this;

        if (SelectionService.Instance != null)
            SelectionService.Instance.EquipToSlot(dragCard.data, slotIndex);

        if (ActivationService.Instance != null)
            ActivationService.Instance.Activate(dragCard.data);
    }

    public void ClearSlot()
    {
        occupied = false;
        currentCard = null;
    }

    public void ClearVisualOnly()
    {
        occupied = false;
        currentCard = null;

        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}