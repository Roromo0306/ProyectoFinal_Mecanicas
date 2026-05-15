using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Radial Weapon")]
public class RadialWeaponEffect : PowerUpEffect
{
    public float orbitRadius = 1.8f;
    public float orbitSpeed = 180f;
    public float damageMultiplier = 1f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableRadialWeapon(orbitRadius, orbitSpeed, damageMultiplier);
    }
}