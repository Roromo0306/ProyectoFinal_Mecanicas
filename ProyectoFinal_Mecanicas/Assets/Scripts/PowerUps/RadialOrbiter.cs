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
        if (playerStats == null) return;
        if (Time.time - lastDamageTime < damageCooldown) return;

        GameObject enemyRoot = GetEnemyRoot(collision.gameObject);
        if (enemyRoot == null) return;

        float damage = playerStats.damage * playerStats.radialDamageMultiplier;

        ApplyHitFeedback(enemyRoot);
        DamageEnemy(enemyRoot, damage);

        lastDamageTime = Time.time;
    }

    private GameObject GetEnemyRoot(GameObject obj)
    {
        if (obj == null) return null;

        EnemyHealthSystem normal = obj.GetComponentInParent<EnemyHealthSystem>();
        if (normal != null)
            return normal.gameObject;

        EliteEnemyHealth elite = obj.GetComponentInParent<EliteEnemyHealth>();
        if (elite != null)
            return elite.gameObject;

        FinalBossHealth boss = obj.GetComponentInParent<FinalBossHealth>();
        if (boss != null)
            return boss.gameObject;

        return null;
    }

    private void DamageEnemy(GameObject enemyRoot, float amount)
    {
        if (enemyRoot == null) return;

        EnemyHealthSystem normal = enemyRoot.GetComponentInParent<EnemyHealthSystem>();
        if (normal != null)
        {
            normal.TakeDamage(amount);
            return;
        }

        EliteEnemyHealth elite = enemyRoot.GetComponentInParent<EliteEnemyHealth>();
        if (elite != null)
        {
            elite.TakeDamage(amount);
            return;
        }

        FinalBossHealth boss = enemyRoot.GetComponentInParent<FinalBossHealth>();
        if (boss != null)
            boss.TakeDamage(amount);
    }

    private void ApplyHitFeedback(GameObject enemyRoot)
    {
        if (enemyRoot == null) return;

        Vector3 knockbackSource = transform.position;

        // Si el arma radial está demasiado cerca del centro del enemigo,
        // usamos la posición del player como origen para que el empuje tenga dirección clara.
        if (Vector2.Distance(transform.position, enemyRoot.transform.position) < 0.15f && player != null)
            knockbackSource = player.position;

        EnemyInstaller normalInstaller = enemyRoot.GetComponent<EnemyInstaller>();
        if (normalInstaller != null)
        {
            normalInstaller.ApplyBulletHitFeedback(knockbackSource, hitKnockbackForce);
            return;
        }

        EliteEnemyController eliteController = enemyRoot.GetComponent<EliteEnemyController>();
        if (eliteController != null)
        {
            eliteController.ApplyBulletHitFeedback(knockbackSource, hitKnockbackForce);
            return;
        }
    }
}