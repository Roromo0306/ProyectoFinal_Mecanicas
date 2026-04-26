using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Explosion")]
public class ExplosionEffect : PowerUpEffect
{
    public float radiusBonus = 2.5f;
    public float damageMultiplier = 1f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableExplodingBullets(radiusBonus, damageMultiplier);
    }
}