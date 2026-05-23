using UnityEngine;

public class ExperienceController
{
    private ExperienceModel model;
    private bool disposed = false;

    private const int MaxLevelUpsPerXPEvent = 20;

    public ExperienceController(ExperienceModel model)
    {
        this.model = model;

        EnsureValidModel();

        EventBus.Subscribe<ExperienceCollectedEvent>(OnXPCollected);

        PublishExperienceState();
    }

    public void Dispose()
    {
        if (disposed)
            return;

        EventBus.Unsubscribe<ExperienceCollectedEvent>(OnXPCollected);
        disposed = true;
    }

    private void OnXPCollected(ExperienceCollectedEvent e)
    {
        if (disposed)
            return;

        EnsureValidModel();

        if (e.amount <= 0)
            return;

        model.currentXP += e.amount;

        int levelUpsThisEvent = 0;

        while (model.currentXP >= model.xpToNextLevel)
        {
            levelUpsThisEvent++;

            if (levelUpsThisEvent > MaxLevelUpsPerXPEvent)
            {
                Debug.LogError(
                    "ExperienceController -> Se han intentado hacer demasiados level ups en un solo evento. " +
                    "Corto el bucle para evitar congelación."
                );

                model.currentXP = Mathf.Clamp(model.currentXP, 0, model.xpToNextLevel - 1);
                break;
            }

            LevelUp();
            EnsureValidModel();
        }

        PublishExperienceState();
    }

    private void LevelUp()
    {
        EnsureValidModel();

        model.currentXP -= model.xpToNextLevel;
        model.currentLevel++;

        model.xpToNextLevel = Mathf.RoundToInt(model.xpToNextLevel * 1.75f);
        model.xpToNextLevel = Mathf.Max(1, model.xpToNextLevel);

        Debug.Log("LEVEL UP LANZADO -> Nivel " + model.currentLevel);

        EventBus.Publish(new LevelUpEvent(model.currentLevel));
    }

    private void PublishExperienceState()
    {
        EnsureValidModel();

        EventBus.Publish(new ExperienceUpdatedEvent(
            model.currentXP,
            model.xpToNextLevel,
            model.currentLevel
        ));
    }

    private void EnsureValidModel()
    {
        if (model == null)
            model = new ExperienceModel();

        if (model.currentLevel < 1)
            model.currentLevel = 1;

        if (model.currentXP < 0)
            model.currentXP = 0;

        if (model.xpToNextLevel <= 0)
        {
            Debug.LogError("ExperienceController -> xpToNextLevel era <= 0. Lo arreglo a 10 para evitar bucle infinito.");
            model.xpToNextLevel = 10;
        }
    }
}