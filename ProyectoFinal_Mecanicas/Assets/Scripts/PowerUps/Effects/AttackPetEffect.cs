using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Attack Pet")]
public class AttackPetEffect : PowerUpEffect
{
    public float damageBonus = 1f;
    public float fireCooldownReduction = 0.05f;
    public float minimumCooldown = 0.08f;

    public float orbitRadius = 2.2f;
    public float orbitSpeed = 160f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableAttackPet(orbitRadius, orbitSpeed);
        stats.AddDamage(damageBonus);
        stats.AddFireRate(fireCooldownReduction, minimumCooldown);
    }
}