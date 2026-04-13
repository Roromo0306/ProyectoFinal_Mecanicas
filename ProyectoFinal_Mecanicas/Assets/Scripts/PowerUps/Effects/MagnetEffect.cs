using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Magnet")]
public class MagnetEffect : PowerUpEffect
{
    public float amount = 2f;

    public override void Apply(PlayerStats stats)
    {
        stats.magnetRadius += amount;
    }
}