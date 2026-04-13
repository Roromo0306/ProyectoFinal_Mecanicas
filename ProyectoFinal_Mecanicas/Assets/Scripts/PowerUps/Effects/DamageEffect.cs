using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Damage")]
public class DamageEffect : PowerUpEffect
{
    public float amount = 1f;

    public override void Apply(PlayerStats stats)
    {
        stats.damage += amount;
    }
}