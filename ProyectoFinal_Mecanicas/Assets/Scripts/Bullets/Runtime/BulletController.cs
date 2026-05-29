using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private const int AreaHitBufferSize = 64;
    private const int MovementHitBufferSize = 16;

    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    [Header("Manual Collision")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float collisionRadius = 0.12f;

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
    private readonly Collider2D[] areaHitsBuffer = new Collider2D[AreaHitBufferSize];
    private readonly RaycastHit2D[] movementHitsBuffer = new RaycastHit2D[MovementHitBufferSize];

    private void Awake()
    {
        if (enemyLayer.value == 0)
            enemyLayer = LayerMask.GetMask("Enemy");
    }

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
        ClearAreaBuffer();
        ClearMovementBuffer();
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

        Vector3 startPosition = transform.position;
        Vector3 movement = data.direction * speed * Time.deltaTime;

        CheckManualHits(startPosition, movement);

        if (!isReleased)
            transform.position = startPosition + movement;

        lifetimeTimer -= Time.deltaTime;

        if (lifetimeTimer <= 0f)
            ReleaseToPool();
    }

    private void CheckManualHits(Vector3 startPosition, Vector3 movement)
    {
        if (isReleased)
            return;

        if (enemyLayer.value == 0)
            return;

        float distance = movement.magnitude;

        if (distance <= 0.001f)
        {
            CheckOverlapAtCurrentPosition();
            return;
        }

        int hitCount = Physics2D.CircleCastNonAlloc(
            startPosition,
            collisionRadius,
            movement.normalized,
            movementHitsBuffer,
            distance,
            enemyLayer
        );

        ProcessMovementHits(hitCount);
        ClearUsedMovementBufferSlots(hitCount);
    }

    private void CheckOverlapAtCurrentPosition()
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            collisionRadius,
            areaHitsBuffer,
            enemyLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            if (isReleased)
                break;

            Collider2D hit = areaHitsBuffer[i];

            if (hit == null)
                continue;

            HandleHit(hit.gameObject);
        }

        ClearUsedAreaBufferSlots(hitCount);
    }

    private void ProcessMovementHits(int hitCount)
    {
        for (int i = 0; i < hitCount; i++)
        {
            if (isReleased)
                break;

            Collider2D hitCollider = movementHitsBuffer[i].collider;

            if (hitCollider == null)
                continue;

            HandleHit(hitCollider.gameObject);
        }
    }

    private void HandleHit(GameObject hitObject)
    {
        if (!isInitialized || isReleased)
            return;

        GameObject enemyRoot = CombatTargetFinder.GetDamageableRoot(hitObject);

        if (!CombatDamageService.IsValidTarget(enemyRoot))
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
        if (!CombatDamageService.IsValidTarget(enemyRoot))
            return;

        ApplyStatuses(enemyRoot);

        if (data.hasExplosion)
            Explode(enemyRoot);

        CombatDamageService.TryApplyHitFeedback(
            enemyRoot,
            transform.position,
            hitKnockbackForce
        );

        CombatDamageService.TryDamage(enemyRoot, data.damage);
    }

    private void ApplyStatuses(GameObject enemyRoot)
    {
        if (!CombatDamageService.IsValidTarget(enemyRoot))
            return;

        if (data.hasFreeze)
        {
            CombatDamageService.TryApplyFreeze(
                enemyRoot,
                data.freezeDuration,
                data.freezeSlowMultiplier
            );
        }

        if (!CombatDamageService.IsValidTarget(enemyRoot))
            return;

        if (data.hasBurn)
        {
            CombatDamageService.TryApplyBurn(
                enemyRoot,
                data.burnDuration,
                data.burnTickDamage,
                data.burnTickInterval
            );
        }
    }

    private void Explode(GameObject mainTarget)
    {
        if (!CombatDamageService.IsValidTarget(mainTarget))
            return;

        Vector3 explosionPosition = mainTarget.transform.position;

        SpawnExplosionParticle(explosionPosition);

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            explosionPosition,
            data.explosionRadius,
            areaHitsBuffer,
            enemyLayer
        );

        float explosionDamage = data.damage * data.explosionDamageMultiplier;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = areaHitsBuffer[i];

            if (hit == null)
                continue;

            GameObject enemyRoot = CombatTargetFinder.GetDamageableRoot(hit.gameObject);

            if (!CombatDamageService.IsValidTarget(enemyRoot))
                continue;

            if (enemyRoot == mainTarget)
                continue;

            ApplyStatuses(enemyRoot);

            if (!CombatDamageService.IsValidTarget(enemyRoot))
                continue;

            CombatDamageService.TryDamage(enemyRoot, explosionDamage);
        }

        ClearUsedAreaBufferSlots(hitCount);
    }

    private GameObject FindNextEnemy(GameObject currentTarget)
    {
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            data.bounceSearchRadius,
            areaHitsBuffer,
            enemyLayer
        );

        GameObject closestEnemy = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = areaHitsBuffer[i];

            if (hit == null)
                continue;

            GameObject enemyRoot = CombatTargetFinder.GetDamageableRoot(hit.gameObject);

            if (!CombatDamageService.IsValidTarget(enemyRoot))
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

        ClearUsedAreaBufferSlots(hitCount);

        return closestEnemy;
    }

    private void ResolvePierceAndBounce(GameObject nextBounceTarget)
    {
        remainingPierceHits--;

        if (CombatDamageService.IsValidTarget(nextBounceTarget) && remainingBounces > 0)
        {
            remainingBounces--;
            data.direction = BulletRuntimeData.NormalizeDirection(nextBounceTarget.transform.position - transform.position);
            return;
        }

        if (remainingPierceHits > 0)
            return;

        ReleaseToPool();
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
        ClearAreaBuffer();
        ClearMovementBuffer();

        BulletObjectPool.Release(gameObject);
    }

    private void ClearUsedAreaBufferSlots(int usedCount)
    {
        for (int i = 0; i < usedCount; i++)
            areaHitsBuffer[i] = null;
    }

    private void ClearAreaBuffer()
    {
        for (int i = 0; i < areaHitsBuffer.Length; i++)
            areaHitsBuffer[i] = null;
    }

    private void ClearUsedMovementBufferSlots(int usedCount)
    {
        for (int i = 0; i < usedCount; i++)
            movementHitsBuffer[i] = default;
    }

    private void ClearMovementBuffer()
    {
        for (int i = 0; i < movementHitsBuffer.Length; i++)
            movementHitsBuffer[i] = default;
    }
}