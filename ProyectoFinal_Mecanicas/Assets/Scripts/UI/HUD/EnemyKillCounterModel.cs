public class EnemyKillCounterModel
{
    public int TotalKills { get; private set; }

    public void Reset()
    {
        TotalKills = 0;
    }

    public void AddKill(int amount = 1)
    {
        if (amount <= 0)
            return;

        TotalKills += amount;
    }
}
