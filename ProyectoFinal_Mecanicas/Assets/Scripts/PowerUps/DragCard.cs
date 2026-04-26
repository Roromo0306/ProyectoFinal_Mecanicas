using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PowerUpData data;

    [Header("Drag Settings")]
    public bool canDrag = true;

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

    private bool CanActuallyDrag()
    {
        if (!canDrag) return false;

        if (deckParent == null && currentSlot == null)
            return false;

        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanActuallyDrag()) return;

        droppedSuccessfully = false;
        isDragging = true;

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        if (currentSlot != null)
        {
            if (SelectionService.Instance != null)
                SelectionService.Instance.RemoveFromSlot(currentSlot.slotIndex);

            currentSlot.ClearSlot();
            currentSlot = null;

            if (ActivationService.Instance != null)
                ActivationService.Instance.RecalculateEquippedPowerUps();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanActuallyDrag()) return;

        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanActuallyDrag()) return;

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

        if (currentSlot != null)
        {
            if (SelectionService.Instance != null)
                SelectionService.Instance.RemoveFromSlot(currentSlot.slotIndex);

            currentSlot.ClearSlot();
            currentSlot = null;
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

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        if (originalSizeSaved)
            rectTransform.sizeDelta = originalSizeDelta;

        droppedSuccessfully = false;

        if (ActivationService.Instance != null)
            ActivationService.Instance.RecalculateEquippedPowerUps();
    }
}