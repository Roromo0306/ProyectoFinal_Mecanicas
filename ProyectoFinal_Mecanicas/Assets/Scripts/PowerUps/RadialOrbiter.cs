using UnityEngine;

public class RadialOrbiter : MonoBehaviour
{
    private Transform player;
    private PlayerStats playerStats;

    private float currentAngle = 0f;
    private float damageCooldown = 0.15f;
    private float lastDamageTime = -999f;

    [Header("Hit Feedback")]
    public float hitKnockbackForce = 2f;

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
        if (playerStats == null)
            return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        GameObject enemyRoot = GetEnemyRoot(collision.gameObject);
        if (enemyRoot == null)
            return;

        float damage = playerStats.damage * playerStats.radialDamageMultiplier;

        ApplyHitFeedback(enemyRoot);
        DamageEnemy(enemyRoot, damage);

        lastDamageTime = Time.time;
    }

    private GameObject GetEnemyRoot(GameObject obj)
    {
        return CombatTargetFinder.GetDamageableRoot(obj);
    }

    private void DamageEnemy(GameObject enemyRoot, float amount)
    {
        if (CombatTargetFinder.TryGetOnRoot(enemyRoot, out IDamageable damageable))
            damageable.TakeDamage(amount);
    }

    private void ApplyHitFeedback(GameObject enemyRoot)
    {
        if (enemyRoot == null)
            return;

        Vector3 knockbackSource = transform.position;

        if (Vector2.Distance(transform.position, enemyRoot.transform.position) < 0.15f && player != null)
            knockbackSource = player.position;

        if (CombatTargetFinder.TryGetOnRoot(enemyRoot, out IHitFeedbackReceiver feedbackReceiver))
            feedbackReceiver.ApplyBulletHitFeedback(knockbackSource, hitKnockbackForce);
    }
}