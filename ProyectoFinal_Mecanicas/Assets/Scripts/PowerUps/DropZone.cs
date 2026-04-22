using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public Transform slotParent;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        var card = draggedObject.GetComponent<DragCard>();
        if (card == null || card.data == null) return;

        card.droppedSuccessfully = true;
        card.transform.SetParent(slotParent != null ? slotParent : transform, false);

        RectTransform rt = card.GetComponent<RectTransform>();
        if (rt != null)
            rt.anchoredPosition = Vector2.zero;

        ActivationService.Instance.Activate(card.data);
    }
}