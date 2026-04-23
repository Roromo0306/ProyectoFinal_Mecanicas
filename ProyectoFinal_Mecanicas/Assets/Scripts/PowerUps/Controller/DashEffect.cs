using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Dash")]
public class DashEffect : PowerUpEffect
{
    public float speed = 18f;
    public float duration = 0.18f;
    public float cooldown = 1.2f;

    public override void Apply(PlayerStats stats)
    {
        stats.hasDash = true;
        stats.dashSpeed = speed;
        stats.dashDuration = duration;
        stats.dashCooldown = cooldown;

        Debug.Log("Dash activado");
    }
}