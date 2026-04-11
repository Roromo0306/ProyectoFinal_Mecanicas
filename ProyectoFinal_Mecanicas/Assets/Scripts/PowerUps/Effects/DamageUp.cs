using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/DamageUp")]
public class DamageUp : PowerUpEffect
{
    public float amount = 1f;

    public override void Apply(PlayerContext context)
    {
        context.stats.damage += amount;
    }
}