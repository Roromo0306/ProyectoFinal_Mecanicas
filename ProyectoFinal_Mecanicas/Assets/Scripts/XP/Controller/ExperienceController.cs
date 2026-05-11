using UnityEngine;

public class ExperienceController
{
    private ExperienceModel model;
    private bool disposed = false;

    public ExperienceController(ExperienceModel model)
    {
        this.model = model;
        EventBus.Subscribe<ExperienceCollectedEvent>(OnXPCollected);

        PublishExperienceState();
    }

    public void Dispose()
    {
        if (disposed) return;

        EventBus.Unsubscribe<ExperienceCollectedEvent>(OnXPCollected);
        disposed = true;
    }

    private void OnXPCollected(object evt)
    {
        if (disposed) return;

        var e = (ExperienceCollectedEvent)evt;

        model.currentXP += e.amount;

        while (model.currentXP >= model.xpToNextLevel)
        {
            LevelUp();
        }

        PublishExperienceState();
    }

    private void LevelUp()
    {
        model.currentXP -= model.xpToNextLevel;
        model.currentLevel++;

        model.xpToNextLevel = Mathf.RoundToInt(model.xpToNextLevel * 1.75f);

        Debug.Log("LEVEL UP LANZADO -> Nivel " + model.currentLevel);
        EventBus.Publish(new LevelUpEvent(model.currentLevel));
    }

    private void PublishExperienceState()
    {
        EventBus.Publish(new ExperienceUpdatedEvent(
            model.currentXP,
            model.xpToNextLevel,
            model.currentLevel
        ));
    }
}