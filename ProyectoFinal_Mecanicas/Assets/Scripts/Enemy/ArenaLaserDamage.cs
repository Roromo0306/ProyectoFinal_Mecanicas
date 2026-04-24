using UnityEngine;

public class ArenaLaserDamage : MonoBehaviour
{
    public float damageCooldown = 0.35f;
    public float pushVelocity = 18f;
    public float pushImpulse = 10f;

    private float lastDamageTime = -999f;
    private Vector3 arenaCenter;

    public void SetArenaCenter(Vector3 center)
    {
        arenaCenter = center;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayer(collision.collider, true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandlePlayer(collision.collider, false);
    }

    private void HandlePlayer(Collider2D collider, bool instant)
    {
        PlayerHealthSystem health = collider.GetComponentInParent<PlayerHealthSystem>();
        if (health == null)
            return;

        Rigidbody2D rb = collider.GetComponentInParent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dirToCenter = ((Vector2)arenaCenter - rb.position).normalized;

            if (instant)
            {
                rb.velocity = dirToCenter * pushVelocity;
                rb.AddForce(dirToCenter * pushImpulse, ForceMode2D.Impulse);
            }
            else
            {
                Vector2 targetVelocity = dirToCenter * pushVelocity;
                rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, 0.65f);
            }
        }

        if (Time.time - lastDamageTime >= damageCooldown)
        {
            lastDamageTime = Time.time;
            health.ForceDamage();
        }
    }
}