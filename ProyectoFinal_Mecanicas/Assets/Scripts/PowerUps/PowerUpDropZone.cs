using UnityEngine;
using UnityEngine.EventSystems;

public class PowerUpDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        PowerUpDragUI card = eventData.pointerDrag.GetComponent<PowerUpDragUI>();

        if (card != null)
        {
            ActivationService.Instance.Activate(card.Data);
        }
    }
}