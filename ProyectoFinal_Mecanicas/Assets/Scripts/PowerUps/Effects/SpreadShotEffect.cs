using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Spread Shot")]
public class SpreadShotEffect : PowerUpEffect
{
    public float angle = 20f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableSpreadShot(angle);
    }
}