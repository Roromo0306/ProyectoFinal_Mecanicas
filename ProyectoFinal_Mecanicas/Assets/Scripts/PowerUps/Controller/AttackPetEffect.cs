using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Attack Pet")]
public class AttackPetEffect : PowerUpEffect
{
    public float damageBonus = 1f;
    public float fireCooldownReduction = 0.05f;
    public float minimumCooldown = 0.08f;

    public override void Apply(PlayerStats stats)
    {
        if (stats.hasAttackPet)
        {
            Debug.Log("La mascota ofensiva ya estaba activa");
            return;
        }

        stats.hasAttackPet = true;
        stats.damage += damageBonus;
        stats.fireCooldown = Mathf.Max(minimumCooldown, stats.fireCooldown - fireCooldownReduction);

        Debug.Log("Mascota ofensiva activada");
    }
}