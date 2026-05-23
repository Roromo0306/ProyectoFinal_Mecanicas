using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardToolTipUI : MonoBehaviour
{
    public static CardToolTipUI Instance;

    [Header("References")]
    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Position")]
    [SerializeField] private Vector2 offset = new Vector2(32f, 36f);
    [SerializeField] private bool keepInsideCanvas = true;
    [SerializeField] private Vector2 canvasPadding = new Vector2(20f, 20f);

    private Canvas canvas;
    private RectTransform canvasRect;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Instance = this;

        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            canvasRect = canvas.transform as RectTransform;

        ConfigureTooltipRaycasts();

        Hide();
    }

    private void Update()
    {
        if (tooltipObject == null)
            return;

        if (!tooltipObject.activeSelf)
            return;

        FollowMouse();
    }

    public void Show(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (tooltipObject == null || tooltipRect == null || descriptionText == null)
            return;

        TMPWaveText waveText = descriptionText.GetComponent<TMPWaveText>();

        if (waveText != null)
            waveText.SetText(text);
        else
            descriptionText.text = text;

        ConfigureTooltipRaycasts();

        tooltipObject.SetActive(true);

        BringTooltipToFront();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);

        FollowMouse();
    }

    public void Hide()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }

    private void FollowMouse()
    {
        if (canvasRect == null)
            return;

        Camera uiCamera = GetUICamera();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            uiCamera,
            out Vector2 localPoint
        );

        Vector2 targetPosition = localPoint + offset;

        if (keepInsideCanvas)
            targetPosition = ClampToCanvas(targetPosition);

        tooltipRect.anchoredPosition = targetPosition;
    }

    private Camera GetUICamera()
    {
        if (canvas == null)
            return null;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return canvas.worldCamera;
    }

    private Vector2 ClampToCanvas(Vector2 targetPosition)
    {
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 tooltipSize = tooltipRect.rect.size;

        float minX = -canvasSize.x * 0.5f + tooltipSize.x * tooltipRect.pivot.x + canvasPadding.x;
        float maxX = canvasSize.x * 0.5f - tooltipSize.x * (1f - tooltipRect.pivot.x) - canvasPadding.x;

        float minY = -canvasSize.y * 0.5f + tooltipSize.y * tooltipRect.pivot.y + canvasPadding.y;
        float maxY = canvasSize.y * 0.5f - tooltipSize.y * (1f - tooltipRect.pivot.y) - canvasPadding.y;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }

    private void BringTooltipToFront()
    {
        if (tooltipObject != null)
            tooltipObject.transform.SetAsLastSibling();

        if (tooltipRect != null)
            tooltipRect.SetAsLastSibling();
    }

    private void ConfigureTooltipRaycasts()
    {
        if (tooltipObject == null)
            return;

        canvasGroup = tooltipObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = tooltipObject.AddComponent<CanvasGroup>();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Graphic[] graphics = tooltipObject.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            graphic.raycastTarget = false;
        }
    }
}