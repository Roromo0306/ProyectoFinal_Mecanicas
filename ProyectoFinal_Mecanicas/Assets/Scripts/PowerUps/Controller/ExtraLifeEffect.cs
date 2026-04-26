using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Extra Life")]
public class ExtraLifeEffect : PowerUpEffect
{
    public int extraLives = 1;

    public override void Apply(PlayerStats stats)
    {
        stats.AddMaxLives(extraLives);
    }
}