using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Hit Feedback")]
    public float hitKnockbackForce = 3f;

    [Header("Hit FX")]
    public GameObject hitParticlePrefab;

    public float speed = 10f;
    public float lifetime = 3f;
    public GameObject explosionParticlePrefab;

    private Vector3 direction;
    private float damage;

    private int remainingPierceHits;
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

    private readonly HashSet<GameObject> hitRoots = new HashSet<GameObject>();

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
        direction = dir.sqrMagnitude <= 0.0001f ? Vector3.right : dir.normalized;
        damage = bulletDamage;

        remainingPierceHits = Mathf.Max(1, pierceCount);
        remainingBounces = Mathf.Max(0, bounceCount);
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
        GameObject enemyRoot = GetEnemyRoot(hitObject);
        if (enemyRoot == null) return;

        if (hitRoots.Contains(enemyRoot)) return;
        hitRoots.Add(enemyRoot);

        GameObject nextBounceTarget = null;

        if (remainingBounces > 0)
            nextBounceTarget = FindNextEnemy(enemyRoot);

        ApplyStatuses(enemyRoot);

        if (hasExplosion)
            Explode(enemyRoot);

        DamageEnemy(enemyRoot, damage);
        ApplyHitFeedback(enemyRoot);

        remainingPierceHits--;

        if (nextBounceTarget != null && remainingBounces > 0)
        {
            remainingBounces--;
            direction = (nextBounceTarget.transform.position - transform.position).normalized;
            return;
        }

        if (remainingPierceHits > 0)
            return;

        Destroy(gameObject);
    }

    private GameObject GetEnemyRoot(GameObject obj)
    {
        if (obj == null) return null;

        EnemyHealthSystem normal = obj.GetComponentInParent<EnemyHealthSystem>();
        if (normal != null) return normal.gameObject;

        EliteEnemyHealth elite = obj.GetComponentInParent<EliteEnemyHealth>();
        if (elite != null) return elite.gameObject;

        FinalBossHealth boss = obj.GetComponentInParent<FinalBossHealth>();
        if (boss != null) return boss.gameObject;

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

    private void ApplyStatuses(GameObject enemyRoot)
    {
        if (enemyRoot == null) return;

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
        if (mainTarget == null) return;

        Vector3 explosionPosition = mainTarget.transform.position;

        if (explosionParticlePrefab != null)
        {
            GameObject fx = Instantiate(explosionParticlePrefab, explosionPosition, Quaternion.identity);
            fx.transform.localScale = Vector3.one * explosionRadius * 0.6f;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);
        float explosionDamage = damage * explosionDamageMultiplier;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject enemyRoot = GetEnemyRoot(hit.gameObject);
            if (enemyRoot == null) continue;
            if (enemyRoot == mainTarget) continue;

            ApplyStatuses(enemyRoot);
            DamageEnemy(enemyRoot, explosionDamage);
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

    private void ApplyHitFeedback(GameObject enemyRoot)
    {
        if (enemyRoot == null) return;

        if (hitParticlePrefab != null)
        {
            Instantiate(hitParticlePrefab, enemyRoot.transform.position, Quaternion.identity);
        }

        EnemyInstaller normalInstaller = enemyRoot.GetComponent<EnemyInstaller>();
        if (normalInstaller != null)
        {
            normalInstaller.ApplyBulletHitFeedback(transform.position, hitKnockbackForce);
            return;
        }

        SpriteRenderer sr = enemyRoot.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            StartCoroutine(FlashWhite(sr));
        }
    }

    private IEnumerator FlashWhite(SpriteRenderer sr)
    {
        if (sr == null) yield break;

        Color originalColor = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(0.08f);

        if (sr != null)
            sr.color = originalColor;
    }
}