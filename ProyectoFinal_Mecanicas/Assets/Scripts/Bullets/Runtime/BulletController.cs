using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    [Header("Hit Feedback")]
    [SerializeField] private float hitKnockbackForce = 3f;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private GameObject explosionParticlePrefab;

    [SerializeField] private float hitFxLifetime = 1.5f;
    [SerializeField] private float explosionFxLifetime = 1.5f;

    private BulletRuntimeData data;

    private int remainingPierceHits;
    private int remainingBounces;

    private float lifetimeTimer;

    private bool isInitialized = false;
    private bool isReleased = false;

    private readonly HashSet<GameObject> hitRoots = new HashSet<GameObject>();
    private static readonly Collider2D[] physicsBuffer = new Collider2D[32];

    public void Init(BulletRuntimeData runtimeData)
    {
        data = runtimeData;
        data.direction = BulletRuntimeData.NormalizeDirection(data.direction);

        remainingPierceHits = Mathf.Max(1, data.pierceCount);
        remainingBounces = Mathf.Max(0, data.bounceCount);

        lifetimeTimer = lifetime;

        isInitialized = true;
        isReleased = false;

        hitRoots.Clear();
    }

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
        BulletRuntimeData runtimeData = new BulletRuntimeData
        {
            direction = dir,
            damage = bulletDamage,

            pierceCount = pierceCount,
            bounceCount = bounceCount,
            bounceSearchRadius = searchRadius,

            hasExplosion = exploding,
            explosionRadius = explosionRadiusValue,
            explosionDamageMultiplier = explosionDamageMultiplierValue,

            hasFreeze = freezing,
            freezeDuration = freezeDurationValue,
            freezeSlowMultiplier = freezeSlowMultiplierValue,

            hasBurn = burning,
            burnDuration = burnDurationValue,
            burnTickDamage = burnTickDamageValue,
            burnTickInterval = burnTickIntervalValue
        };

        Init(runtimeData);
    }

    private void Update()
    {
        if (!isInitialized || isReleased)
            return;

        transform.position += data.direction * speed * Time.deltaTime;

        lifetimeTimer -= Time.deltaTime;

        if (lifetimeTimer <= 0f)
            ReleaseToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized || isReleased)
            return;

        HandleHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInitialized || isReleased)
            return;

        HandleHit(collision.gameObject);
    }

    private void HandleHit(GameObject hitObject)
    {
        GameObject enemyRoot = GetEnemyRoot(hitObject);

        if (enemyRoot == null)
            return;

        if (hitRoots.Contains(enemyRoot))
            return;

        hitRoots.Add(enemyRoot);

        SFXManager.Instance?.PlayEnemyHit();
        SpawnHitParticle(enemyRoot.transform.position);

        GameObject nextBounceTarget = null;

        if (remainingBounces > 0)
            nextBounceTarget = FindNextEnemy(enemyRoot);

        ApplyDirectHit(enemyRoot);
        ResolvePierceAndBounce(nextBounceTarget);
    }

    private void ApplyDirectHit(GameObject enemyRoot)
    {
        ApplyStatuses(enemyRoot);

        if (data.hasExplosion)
            Explode(enemyRoot);

        DamageEnemy(enemyRoot, data.damage);
        ApplyHitFeedback(enemyRoot);
    }

    private void ResolvePierceAndBounce(GameObject nextBounceTarget)
    {
        remainingPierceHits--;

        if (nextBounceTarget != null && remainingBounces > 0)
        {
            remainingBounces--;
            data.direction = BulletRuntimeData.NormalizeDirection(nextBounceTarget.transform.position - transform.position);
            return;
        }

        if (remainingPierceHits > 0)
            return;

        ReleaseToPool();
    }

    private GameObject GetEnemyRoot(GameObject obj)
    {
        return CombatTargetFinder.GetDamageableRoot(obj);
    }

    private void DamageEnemy(GameObject enemyRoot, float amount)
    {
        if (enemyRoot == null)
            return;

        if (CombatTargetFinder.TryGetOnRoot(enemyRoot, out IDamageable damageable))
            damageable.TakeDamage(amount);
    }

    private void ApplyStatuses(GameObject enemyRoot)
    {
        if (enemyRoot == null)
            return;

        if (data.hasFreeze && CombatTargetFinder.TryGetOnRoot(enemyRoot, out IFreezable freezable))
            freezable.ApplyFreeze(data.freezeDuration, data.freezeSlowMultiplier);

        if (data.hasBurn && CombatTargetFinder.TryGetOnRoot(enemyRoot, out IBurnable burnable))
            burnable.ApplyBurn(data.burnDuration, data.burnTickDamage, data.burnTickInterval);
    }

    private void Explode(GameObject mainTarget)
    {
        if (mainTarget == null)
            return;

        Vector3 explosionPosition = mainTarget.transform.position;

        SpawnExplosionParticle(explosionPosition);

        int hitCount = Physics2D.OverlapCircleNonAlloc(explosionPosition, data.explosionRadius, physicsBuffer);
        float explosionDamage = data.damage * data.explosionDamageMultiplier;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = physicsBuffer[i];

            if (hit == null)
                continue;

            GameObject enemyRoot = GetEnemyRoot(hit.gameObject);

            if (enemyRoot == null)
                continue;

            if (enemyRoot == mainTarget)
                continue;

            ApplyStatuses(enemyRoot);
            DamageEnemy(enemyRoot, explosionDamage);
        }
    }

    private GameObject FindNextEnemy(GameObject currentTarget)
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, data.bounceSearchRadius, physicsBuffer);

        GameObject closestEnemy = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = physicsBuffer[i];

            if (hit == null)
                continue;

            GameObject enemyRoot = GetEnemyRoot(hit.gameObject);

            if (enemyRoot == null)
                continue;

            if (enemyRoot == currentTarget)
                continue;

            if (hitRoots.Contains(enemyRoot))
                continue;

            float sqrDistance = (enemyRoot.transform.position - transform.position).sqrMagnitude;

            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestEnemy = enemyRoot;
            }
        }

        return closestEnemy;
    }

    private void ApplyHitFeedback(GameObject enemyRoot)
    {
        if (enemyRoot == null)
            return;

        if (CombatTargetFinder.TryGetOnRoot(enemyRoot, out IHitFeedbackReceiver feedbackReceiver))
            feedbackReceiver.ApplyBulletHitFeedback(transform.position, hitKnockbackForce);
    }

    private void SpawnHitParticle(Vector3 position)
    {
        if (hitParticlePrefab == null)
            return;

        VFXObjectPool.Spawn(
            hitParticlePrefab,
            position,
            Quaternion.identity,
            hitFxLifetime
        );
    }

    private void SpawnExplosionParticle(Vector3 position)
    {
        if (explosionParticlePrefab == null)
            return;

        Vector3 scale = Vector3.one * data.explosionRadius * 0.6f;

        VFXObjectPool.Spawn(
            explosionParticlePrefab,
            position,
            Quaternion.identity,
            scale,
            explosionFxLifetime
        );
    }

    private void ReleaseToPool()
    {
        if (isReleased)
            return;

        isReleased = true;
        isInitialized = false;

        hitRoots.Clear();

        BulletObjectPool.Release(gameObject);
    }
}