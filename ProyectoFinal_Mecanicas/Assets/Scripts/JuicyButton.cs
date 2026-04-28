using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JuicyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale")]
    public float hoverScale = 1.08f;
    public float clickScale = 0.95f;
    public float animationSpeed = 12f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine scaleRoutine;

    private Button button;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        SetTargetScale(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetTargetScale(originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        SetTargetScale(originalScale * clickScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        SetTargetScale(originalScale * hoverScale);
    }

    private void SetTargetScale(Vector3 newScale)
    {
        targetScale = newScale;

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine()
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.unscaledDeltaTime * animationSpeed
            );

            yield return null;
        }

        transform.localScale = targetScale;
    }
}