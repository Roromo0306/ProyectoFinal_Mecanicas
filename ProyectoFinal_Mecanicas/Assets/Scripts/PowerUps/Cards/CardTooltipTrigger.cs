using UnityEngine;
using UnityEngine.EventSystems;

public class CardTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PowerUpData data;

    private DragCard dragCard;

    private void Awake()
    {
        dragCard = GetComponent<DragCard>();
    }

    public void SetData(PowerUpData newData)
    {
        data = newData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (data == null) return;

        if (dragCard != null && dragCard.IsDragging())
            return;

        if (CardToolTipUI.Instance != null)
            CardToolTipUI.Instance.Show(data.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardToolTipUI.Instance != null)
            CardToolTipUI.Instance.Hide();
    }

    private void OnDisable()
    {
        if (CardToolTipUI.Instance != null)
            CardToolTipUI.Instance.Hide();
    }
}