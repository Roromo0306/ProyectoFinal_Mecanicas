public struct ExperienceUpdatedEvent
{
    public int currentXP;
    public int xpToNextLevel;
    public int currentLevel;

    public ExperienceUpdatedEvent(int currentXP, int xpToNextLevel, int currentLevel)
    {
        this.currentXP = currentXP;
        this.xpToNextLevel = xpToNextLevel;
        this.currentLevel = currentLevel;
    }
}