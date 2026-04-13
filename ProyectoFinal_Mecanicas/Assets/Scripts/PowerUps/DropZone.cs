using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag.GetComponent<DragCard>();

        if (card == null) return;

        ActivationService.Instance.Activate(card.data);

        Destroy(card.gameObject);
    }
}