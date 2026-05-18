using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceView : MonoBehaviour
{
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    [Header("XP Text")]
    public TextMeshProUGUI xpText;

    [Header("Smooth Bar")]
    public float smoothSpeed = 8f;

    private float targetXPValue = 0f;
    private int currentXP = 0;
    private int xpToNext = 10;
    private int currentLevel = 1;

    private void OnEnable()
    {
        EventBus.Subscribe<ExperienceUpdatedEvent>(OnExperienceUpdated);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ExperienceUpdatedEvent>(OnExperienceUpdated);
    }

    private void Start()
    {
        RefreshInstant();
    }

    private void Update()
    {
        if (xpBar == null) return;

        xpBar.value = Mathf.Lerp(
            xpBar.value,
            targetXPValue,
            Time.unscaledDeltaTime * smoothSpeed
        );

        if (Mathf.Abs(xpBar.value - targetXPValue) < 0.01f)
            xpBar.value = targetXPValue;
    }

    private void OnExperienceUpdated(ExperienceUpdatedEvent e)
    {
        currentXP = e.currentXP;
        xpToNext = e.xpToNextLevel;
        currentLevel = e.currentLevel;

        if (xpBar != null)
        {
            xpBar.minValue = 0;
            xpBar.maxValue = xpToNext;
            targetXPValue = currentXP;
        }

        if (levelText != null)
            levelText.text = "Level " + currentLevel;

        if (xpText != null)
            xpText.text = "EXP - " + currentXP + "/" + xpToNext;
    }

    private void RefreshInstant()
    {
        if (xpBar != null)
        {
            xpBar.minValue = 0;
            xpBar.maxValue = xpToNext;
            xpBar.value = currentXP;
            targetXPValue = currentXP;
        }

        if (levelText != null)
            levelText.text = "Level " + currentLevel;

        if (xpText != null)
            xpText.text = "EXP - " + currentXP + "/" + xpToNext;
    }
}