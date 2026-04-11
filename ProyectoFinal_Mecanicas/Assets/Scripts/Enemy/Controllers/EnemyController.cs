using UnityEngine;

public class EnemyController
{
    

    private Transform transform;
    private Transform player;

    private Vector2 knockbackVelocity;
    private float knockbackDamping = 8f;
    private float knockbackForce = 30f;

    public EnemyController(Transform transform)
    {
        this.transform = transform;
        player = GameObject.FindWithTag("Player").transform;
    }

    public void Tick()
    {
        Move();
        ApplyKnockback();
    }

    private void Move()
    {
        var dir = (player.position - transform.position).normalized;
        transform.position += dir * 2f * Time.deltaTime;
    }

    private void ApplyKnockback()
    {
        transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackDamping * Time.deltaTime);
    }

    public void OnPlayerCollision(Transform playerTransform)
    {
        EventBus.Publish(new PlayerHitEvent());

        Vector2 dir = (transform.position - playerTransform.position).normalized;
        knockbackVelocity = dir * knockbackForce; // fuerza del empuje
    }

    public void ApplyRadialKnockback(Vector3 hitPosition, float radius, float force)
    {
        Vector2 dir = (transform.position - hitPosition);
        float distance = dir.magnitude;

        if (distance > radius)
            return;

        float falloff = 1.5f - (distance / radius);

        knockbackVelocity = dir.normalized * knockbackForce * falloff;
    }
}