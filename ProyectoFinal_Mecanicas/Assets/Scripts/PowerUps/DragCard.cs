using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PowerUpData data;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [HideInInspector] public bool droppedSuccessfully = false;
    [HideInInspector] public CardSlot currentSlot = null;
    [HideInInspector] public Transform deckParent = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        droppedSuccessfully = false;

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        if (currentSlot != null)
        {
            currentSlot.ClearSlot();
            currentSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        if (!droppedSuccessfully)
        {
            ReturnToDeck();
        }
    }

    public void ReturnToDeck()
    {
        if (deckParent == null)
        {
            Debug.LogError("DragCard -> deckParent es null");
            return;
        }

        transform.SetParent(deckParent, false);

        LayoutElement le = GetComponent<LayoutElement>();
        if (le != null)
            le.ignoreLayout = false;

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        currentSlot = null;
    }
}