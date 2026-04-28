using UnityEngine;
using TMPro;

public class CardToolTipUI : MonoBehaviour
{
    public static CardToolTipUI Instance;

    [Header("References")]
    public GameObject tooltipObject;
    public RectTransform tooltipRect;
    public TextMeshProUGUI descriptionText;

    [Header("Position")]
    public Vector2 offset = new Vector2(0f, 120f);

    private Canvas canvas;

    private void Awake()
    {
        Instance = this;
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    private void Update()
    {
        if (tooltipObject != null && tooltipObject.activeSelf)
            FollowMouse();
    }

    public void Show(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        TMPWaveText wave = descriptionText.GetComponent<TMPWaveText>();
        wave.SetText(text);
        tooltipObject.SetActive(true);

        Canvas.ForceUpdateCanvases();
        FollowMouse();
    }

    public void Hide()
    {
        if (tooltipObject != null)
            tooltipObject.SetActive(false);
    }

    private void FollowMouse()
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out localPoint
        );

        tooltipRect.anchoredPosition = localPoint + offset;
    }
}