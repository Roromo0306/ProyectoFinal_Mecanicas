using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Pierce")]
public class PierceEffect : PowerUpEffect
{
    public int amount = 1;

    public override void Apply(PlayerStats stats)
    {
        stats.pierceCount += amount;
    }
}