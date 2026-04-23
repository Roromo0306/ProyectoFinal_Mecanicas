using UnityEngine;

public class RadialOrbiter : MonoBehaviour
{
    private Transform player;
    private PlayerStats playerStats;

    private float currentAngle = 0f;
    private float damageCooldown = 0.15f;
    private float lastDamageTime = -999f;

    public void Init(Transform playerTransform, PlayerStats stats, float startAngle)
    {
        player = playerTransform;
        playerStats = stats;
        currentAngle = startAngle;
    }

    private void Update()
    {
        if (player == null || playerStats == null)
            return;

        currentAngle += playerStats.radialOrbitSpeed * Time.deltaTime;

        float radians = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians),
            0f
        ) * playerStats.radialOrbitRadius;

        transform.position = player.position + offset;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerStats == null) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        EnemyHealthSystem enemy = collision.GetComponent<EnemyHealthSystem>();
        if (enemy == null) return;

        float damage = playerStats.damage * playerStats.radialDamageMultiplier;
        enemy.TakeDamage(damage);

        lastDamageTime = Time.time;
    }
}