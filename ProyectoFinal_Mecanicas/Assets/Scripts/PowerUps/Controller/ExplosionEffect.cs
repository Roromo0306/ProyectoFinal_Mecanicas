using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Explosion")]
public class ExplosionEffect : PowerUpEffect
{
    public float radiusBonus = 2.5f;
    public float damageMultiplier = 1f;

    public override void Apply(PlayerStats stats)
    {
        stats.hasExplodingBullets = true;
        stats.explosionRadius = radiusBonus;
        stats.explosionDamageMultiplier = damageMultiplier;

        //Debug.Log("Exploding Bullets activado -> radio: " + stats.explosionRadius);
    }
}