using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyKillCounterView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private RectTransform popTarget;

    [Header("Text")]
    [SerializeField] private string prefix = "KILLS - ";

    [Header("Pop Animation")]
    [SerializeField] private float popScale = 1.18f;
    [SerializeField] private float popUpDuration = 0.08f;
    [SerializeField] private float popDownDuration = 0.12f;

    private Vector3 baseScale = Vector3.one;
    private Coroutine popRoutine;
    private int lastCount = 0;

    private void Awake()
    {
        if (counterText == null)
            counterText = GetComponent<TextMeshProUGUI>();

        if (popTarget == null && counterText != null)
            popTarget = counterText.rectTransform;

        if (popTarget != null)
            baseScale = popTarget.localScale;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyKillCountUpdatedEvent>(OnKillCountUpdated);
        RefreshText(lastCount, false);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyKillCountUpdatedEvent>(OnKillCountUpdated);

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            popRoutine = null;
        }

        ResetPopScale();
    }

    private void OnKillCountUpdated(EnemyKillCountUpdatedEvent e)
    {
        bool shouldPop = e.totalKills > lastCount;
        lastCount = e.totalKills;

        RefreshText(lastCount, shouldPop);
    }

    private void RefreshText(int amount, bool playPop)
    {
        if (counterText != null)
            counterText.text = prefix + amount;

        if (playPop)
            PlayPop();
    }

    private void PlayPop()
    {
        if (popTarget == null)
            return;

        if (popRoutine != null)
            StopCoroutine(popRoutine);

        popRoutine = StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        Vector3 startScale = baseScale;
        Vector3 targetScale = baseScale * popScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / popUpDuration;
            popTarget.localScale = Vector3.Lerp(startScale, targetScale, EaseOutBack(t));
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / popDownDuration;
            popTarget.localScale = Vector3.Lerp(targetScale, baseScale, EaseOutBack(t));
            yield return null;
        }

        ResetPopScale();
        popRoutine = null;
    }

    private void ResetPopScale()
    {
        if (popTarget != null)
            popTarget.localScale = baseScale;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
