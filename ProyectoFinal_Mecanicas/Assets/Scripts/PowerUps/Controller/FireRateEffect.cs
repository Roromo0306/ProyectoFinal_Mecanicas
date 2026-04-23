using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Fire Rate")]
public class FireRateEffect : PowerUpEffect
{
    public float cooldownReduction = 0.05f;
    public float minimumCooldown = 0.08f;

    public override void Apply(PlayerStats stats)
    {
        stats.fireCooldown -= cooldownReduction;
        stats.fireCooldown = Mathf.Max(stats.fireCooldown, minimumCooldown);

        Debug.Log("Fire Rate activado -> nuevo cooldown: " + stats.fireCooldown);
    }
}