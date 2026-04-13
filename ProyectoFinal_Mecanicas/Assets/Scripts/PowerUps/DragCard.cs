using UnityEngine;
using UnityEngine.EventSystems;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PowerUpData data;

    private RectTransform rect;
    private CanvasGroup group;
    private Transform parent;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        parent = transform.parent;
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        rect.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData e)
    {
        group.blocksRaycasts = true;
        transform.SetParent(parent);
    }
}