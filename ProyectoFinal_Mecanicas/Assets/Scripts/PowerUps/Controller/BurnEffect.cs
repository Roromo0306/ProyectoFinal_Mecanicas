using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Burn")]
public class BurnEffect : PowerUpEffect
{
    public float duration = 3f;
    public float tickDamage = 0.2f;
    public float tickInterval = 0.4f;

    public override void Apply(PlayerStats stats)
    {
        stats.hasBurnBullets = true;
        stats.burnDuration = duration;
        stats.burnTickDamage = tickDamage;
        stats.burnTickInterval = tickInterval;

        Debug.Log("Burn activado -> duración: " + duration);
    }
}