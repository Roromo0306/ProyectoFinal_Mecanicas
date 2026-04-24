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

    private HashSet<GameObject> hitRoots = new HashSet<GameObject>();

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
        HandleHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    private void HandleHit(GameObject hitObject)
    {
        if (hitObject == null) return;

        GameObject enemyRoot = GetEnemyRoot(hitObject);
        if (enemyRoot == null) return;

        if (hitRoots.Contains(enemyRoot))
            return;

        hitRoots.Add(enemyRoot);

        DamageEnemy(enemyRoot, damage);
        ApplyStatuses(enemyRoot);

        if (hasExplosion)
            Explode(enemyRoot);

        remainingHits--;

        if (remainingBounces > 0)
        {
            GameObject nextTarget = FindNextEnemy(enemyRoot);

            if (nextTarget != null)
            {
                remainingBounces--;
                direction = (nextTarget.transform.position - transform.position).normalized;
            }
        }

        if (remainingHits <= 0)
            Destroy(gameObject);
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
    }

    private void ApplyStatuses(GameObject enemyRoot)
    {
        if (hasFreeze)
        {
            EnemyInstaller normalInstaller = enemyRoot.GetComponent<EnemyInstaller>();
            if (normalInstaller != null)
                normalInstaller.ApplyFreeze(freezeDuration, freezeSlowMultiplier);

            EliteEnemyController eliteController = enemyRoot.GetComponent<EliteEnemyController>();
            if (eliteController != null)
                eliteController.ApplyFreeze(freezeDuration, freezeSlowMultiplier);
        }

        if (hasBurn)
        {
            EnemyInstaller normalInstaller = enemyRoot.GetComponent<EnemyInstaller>();
            if (normalInstaller != null)
                normalInstaller.ApplyBurn(burnDuration, burnTickDamage, burnTickInterval);

            EliteEnemyController eliteController = enemyRoot.GetComponent<EliteEnemyController>();
            if (eliteController != null)
                eliteController.ApplyBurn(burnDuration, burnTickDamage, burnTickInterval);
        }
    }

    private void Explode(GameObject mainTarget)
    {
        if (explosionParticlePrefab != null)
        {
            GameObject fx = Instantiate(explosionParticlePrefab, mainTarget.transform.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * explosionRadius * 0.6f;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(mainTarget.transform.position, explosionRadius);
        float explosionDamage = damage * explosionDamageMultiplier;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject enemyRoot = GetEnemyRoot(hit.gameObject);
            if (enemyRoot == null) continue;
            if (enemyRoot == mainTarget) continue;

            DamageEnemy(enemyRoot, explosionDamage);
            ApplyStatuses(enemyRoot);
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

            GameObject enemyRoot = GetEnemyRoot(hit.gameObject);
            if (enemyRoot == null) continue;
            if (enemyRoot == currentTarget) continue;
            if (hitRoots.Contains(enemyRoot)) continue;

            float distance = Vector2.Distance(transform.position, enemyRoot.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyRoot;
            }
        }

        return closestEnemy;
    }
}