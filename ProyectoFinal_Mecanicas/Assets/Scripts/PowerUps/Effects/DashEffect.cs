using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Dash")]
public class DashEffect : PowerUpEffect
{
    public float speed = 18f;
    public float duration = 0.18f;
    public float cooldown = 1.2f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableDash(speed, duration, cooldown);
    }
}