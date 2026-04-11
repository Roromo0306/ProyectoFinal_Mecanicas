using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/FireRateUp")]
public class FireRateUp : PowerUpEffect
{
    public float multiplier = 1.2f;

    public override void Apply(PlayerContext context)
    {
        context.stats.fireRate *= multiplier;
    }
}