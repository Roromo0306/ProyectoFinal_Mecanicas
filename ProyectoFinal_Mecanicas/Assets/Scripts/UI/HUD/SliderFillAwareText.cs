using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderFillAwareText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform referenceRect;

    [Header("Text Layers")]
    [SerializeField] private TextMeshProUGUI normalText;
    [SerializeField] private TextMeshProUGUI filledText;

    [Header("Mask")]
    [SerializeField] private RectTransform filledTextMask;

    [Header("Colors")]
    [SerializeField] private bool forceColors = true;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color filledTextColor = Color.black;

    [Header("Behaviour")]
    [SerializeField] private bool syncTextAutomatically = true;
    [SerializeField] private bool autoConfigureRects = true;

    private void Awake()
    {
        CacheReferences();
        ConfigureRaycasts();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void OnValidate()
    {
        CacheReferences();
        Refresh();
    }

    private void CacheReferences()
    {
        if (slider == null)
            slider = GetComponentInParent<Slider>();

        if (referenceRect == null && slider != null)
            referenceRect = slider.transform as RectTransform;
    }

    private void Refresh()
    {
        if (slider == null || referenceRect == null || filledTextMask == null)
            return;

        if (normalText == null || filledText == null)
            return;

        if (syncTextAutomatically)
            filledText.text = normalText.text;

        if (forceColors)
        {
            normalText.color = normalTextColor;
            filledText.color = filledTextColor;
        }

        if (autoConfigureRects)
            ConfigureRects();

        float normalizedValue = GetNormalizedSliderValue();
        float fullWidth = referenceRect.rect.width;
        float visibleWidth = fullWidth * normalizedValue;

        filledTextMask.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            visibleWidth
        );
    }

    private float GetNormalizedSliderValue()
    {
        if (slider.maxValue <= slider.minValue)
            return 0f;

        return Mathf.InverseLerp(
            slider.minValue,
            slider.maxValue,
            slider.value
        );
    }

    private void ConfigureRects()
    {
        RectTransform normalTextRect = normalText.rectTransform;
        RectTransform filledTextRect = filledText.rectTransform;

        normalTextRect.anchorMin = Vector2.zero;
        normalTextRect.anchorMax = Vector2.one;
        normalTextRect.pivot = new Vector2(0.5f, 0.5f);
        normalTextRect.offsetMin = Vector2.zero;
        normalTextRect.offsetMax = Vector2.zero;

        filledTextMask.anchorMin = new Vector2(0f, 0f);
        filledTextMask.anchorMax = new Vector2(0f, 1f);
        filledTextMask.pivot = new Vector2(0f, 0.5f);
        filledTextMask.anchoredPosition = Vector2.zero;
        filledTextMask.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            referenceRect.rect.height
        );

        filledTextRect.anchorMin = new Vector2(0f, 0f);
        filledTextRect.anchorMax = new Vector2(0f, 1f);
        filledTextRect.pivot = new Vector2(0f, 0.5f);
        filledTextRect.anchoredPosition = Vector2.zero;

        filledTextRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            referenceRect.rect.width
        );

        filledTextRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            referenceRect.rect.height
        );
    }

    private void ConfigureRaycasts()
    {
        if (normalText != null)
            normalText.raycastTarget = false;

        if (filledText != null)
            filledText.raycastTarget = false;
    }
}