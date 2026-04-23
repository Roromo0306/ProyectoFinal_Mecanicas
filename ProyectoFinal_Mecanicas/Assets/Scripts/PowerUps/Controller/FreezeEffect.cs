using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Freeze")]
public class FreezeEffect : PowerUpEffect
{
    public float duration = 2f;
    public float slowMultiplier = 0.4f;

    public override void Apply(PlayerStats stats)
    {
        stats.hasFreezeBullets = true;
        stats.freezeDuration = duration;
        stats.freezeSlowMultiplier = slowMultiplier;

        Debug.Log("Freeze activado -> duración: " + duration);
    }
}