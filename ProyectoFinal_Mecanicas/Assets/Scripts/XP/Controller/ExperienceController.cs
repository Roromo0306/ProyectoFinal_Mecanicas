using UnityEngine;

public class ExperienceController
{
    private ExperienceModel model;

    public ExperienceController(ExperienceModel model)
    {
        this.model = model;

        EventBus.Subscribe<ExperienceCollectedEvent>(OnXPCollected);
    }

    private void OnXPCollected(object evt)
    {
        var e = (ExperienceCollectedEvent)evt;

        model.currentXP += e.amount;

        if (model.currentXP >= model.xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        model.currentXP -= model.xpToNextLevel;
        model.currentLevel++;

        model.xpToNextLevel = Mathf.RoundToInt(model.xpToNextLevel * 1.5f);

        EventBus.Publish(new LevelUpEvent(model.currentLevel));
    }
}