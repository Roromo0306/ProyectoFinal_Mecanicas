using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceView : MonoBehaviour
{
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    private void OnEnable()
    {
        EventBus.Subscribe<ExperienceCollectedEvent>(OnXP);
        EventBus.Subscribe<LevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ExperienceCollectedEvent>(OnXP);
        EventBus.Unsubscribe<LevelUpEvent>(OnLevelUp);
    }

    private int currentXP = 0;
    private int xpToNext = 10;

    private void OnXP(object evt)
    {
        var e = (ExperienceCollectedEvent)evt;

        currentXP += e.amount;
        xpBar.value = (float)currentXP / xpToNext;
    }

    private void OnLevelUp(object evt)
    {
        var e = (LevelUpEvent)evt;

        currentXP = 0;
        xpToNext = Mathf.RoundToInt(xpToNext * 1.5f);

        levelText.text = "Level " + e.newLevel;
        xpBar.value = 0;
    }
}