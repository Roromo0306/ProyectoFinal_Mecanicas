public struct EnemyKillCountUpdatedEvent
{
    public int totalKills;

    public EnemyKillCountUpdatedEvent(int totalKills)
    {
        this.totalKills = totalKills;
    }
}
