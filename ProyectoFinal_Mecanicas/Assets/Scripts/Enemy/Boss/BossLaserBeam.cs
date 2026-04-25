using UnityEngine;

public class BossLaserBeam : MonoBehaviour
{
    public float damageCooldown = 0.3f;
    private float lastDamageTime = -999f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHealthSystem player = collision.GetComponentInParent<PlayerHealthSystem>();

        if (player == null)
            return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        player.ForceDamage();
    }
}