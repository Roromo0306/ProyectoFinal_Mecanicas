using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    public GameObject explosionParticlePrefab;

    private Vector3 direction;
    private float damage;
    private int remainingHits;
    private int remainingBounces;
    private float bounceSearchRadius;

    private bool hasExplosion;
    private float explosionRadius;
    private float explosionDamageMultiplier;

    private bool hasFreeze;
    private float freezeDuration;
    private float freezeSlowMultiplier;

    private bool hasBurn;
    private float burnDuration;
    private float burnTickDamage;
    private float burnTickInterval;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void Init(
        Vector3 dir,
        float bulletDamage,
        int pierceCount,
        int bounceCount,
        float searchRadius,
        bool exploding,
        float explosionRadiusValue,
        float explosionDamageMultiplierValue,
        bool freezing,
        float freezeDurationValue,
        float freezeSlowMultiplierValue,
        bool burning,
        float burnDurationValue,
        float burnTickDamageValue,
        float burnTickIntervalValue
    )
    {
        direction = dir.normalized;
        damage = bulletDamage;

        remainingHits = Mathf.Max(pierceCount, bounceCount + 1);
        remainingBounces = bounceCount;
        bounceSearchRadius = searchRadius;

        hasExplosion = exploding;
        explosionRadius = explosionRadiusValue;
        explosionDamageMultiplier = explosionDamageMultiplierValue;

        hasFreeze = freezing;
        freezeDuration = freezeDurationValue;
        freezeSlowMultiplier = freezeSlowMultiplierValue;

        hasBurn = burning;
        burnDuration = burnDurationValue;
        burnTickDamage = burnTickDamageValue;
        burnTickInterval = burnTickIntervalValue;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        GameObject target = collision.gameObject;

        if (hitEnemies.Contains(target))
            return;

        EnemyHealthSystem enemy = target.GetComponent<EnemyHealthSystem>();
        if (enemy == null)
            return;

        hitEnemies.Add(target);

        EventBus.Publish(new EnemyHitEvent(target, damage));

        if (hasFreeze)
            ApplyFreeze(target);

        if (hasBurn)
            ApplyBurn(target);

        if (hasExplosion)
            Explode(target);

        remainingHits--;

        if (remainingBounces > 0)
        {
            GameObject nextTarget = FindNextEnemy(target);

            if (nextTarget != null)
            {
                remainingBounces--;

                Vector3 newDir = (nextTarget.transform.position - transform.position).normalized;
                direction = newDir;
            }
        }

        if (remainingHits <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ApplyFreeze(GameObject target)
    {
        EnemyInstaller enemyInstaller = target.GetComponent<EnemyInstaller>();
        if (enemyInstaller != null)
        {
            enemyInstaller.ApplyFreeze(freezeDuration, freezeSlowMultiplier);
        }
    }

    private void ApplyBurn(GameObject target)
    {
        EnemyInstaller enemyInstaller = target.GetComponent<EnemyInstaller>();
        if (enemyInstaller != null)
        {
            enemyInstaller.ApplyBurn(burnDuration, burnTickDamage, burnTickInterval);
        }
    }

    private void Explode(GameObject mainTarget)
    {
        if (explosionParticlePrefab != null)
        {
            GameObject fx = Instantiate(
                explosionParticlePrefab,
                mainTarget.transform.position,
                Quaternion.identity
            );

            float scale = explosionRadius * 0.6f;
            fx.transform.localScale = Vector3.one * scale;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(mainTarget.transform.position, explosionRadius);

        float explosionDamage = damage * explosionDamageMultiplier;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject candidate = hit.gameObject;

            if (candidate == mainTarget) continue;

            EnemyHealthSystem enemy = candidate.GetComponent<EnemyHealthSystem>();
            if (enemy == null) continue;

            EventBus.Publish(new EnemyHitEvent(candidate, explosionDamage));

            if (hasFreeze)
                ApplyFreeze(candidate);

            if (hasBurn)
                ApplyBurn(candidate);
        }
    }

    private GameObject FindNextEnemy(GameObject currentTarget)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceSearchRadius);

        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject candidate = hit.gameObject;

            if (candidate == currentTarget) continue;
            if (hitEnemies.Contains(candidate)) continue;

            EnemyHealthSystem enemy = candidate.GetComponent<EnemyHealthSystem>();
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, candidate.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = candidate;
            }
        }

        return closestEnemy;
    }
}