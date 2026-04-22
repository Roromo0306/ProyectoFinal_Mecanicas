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

    private Vector2 originalSizeDelta;
    private bool originalSizeSaved = false;

    private bool isDragging;


    public bool IsDragging()
    {
        return isDragging;
    }


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalSizeDelta = rectTransform.sizeDelta;
        originalSizeSaved = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        droppedSuccessfully = false;
        isDragging = true;

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
        isDragging = false;

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
        LayoutElement le = GetComponent<LayoutElement>();
        if (le != null)
        {
            le.ignoreLayout = false;
            le.preferredWidth = 180;
            le.preferredHeight = 240;
        }
        transform.SetParent(deckParent, false);

        CardBalatroVisual visual = GetComponent<CardBalatroVisual>();
        if (visual != null)
            visual.ResetVisual();

        if (le != null)
            le.ignoreLayout = false;

       

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        if (originalSizeSaved)
            rectTransform.sizeDelta = originalSizeDelta;

        currentSlot = null;
    }
}