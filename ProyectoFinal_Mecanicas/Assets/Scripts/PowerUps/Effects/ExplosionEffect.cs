using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Explosion")]
public class ExplosionEffect : PowerUpEffect
{
    public float radiusBonus = 1.15f;
    public float damageMultiplier = 0.35f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableExplodingBullets(radiusBonus, damageMultiplier);
    }
}