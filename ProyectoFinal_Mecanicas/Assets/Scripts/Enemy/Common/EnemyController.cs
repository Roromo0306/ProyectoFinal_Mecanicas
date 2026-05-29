using UnityEngine;

public class EnemyController
{
    private const int SeparationBufferSize = 16;

    private Transform enemyTransform;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private EnemyHealthSystem cachedHealth;

    private readonly Collider2D[] separationHitsBuffer = new Collider2D[SeparationBufferSize];

    private float baseSpeed = 2f;
    private float knockbackForce = 0f;
    private Vector2 knockbackVelocity;

    private float playerContactRadius = 0.45f;
    private float playerHitCooldown = 0.5f;
    private float playerHitTimer = 0f;

    private float separationRadius = 0.6f;
    private float separationForce = 2.5f;
    private LayerMask enemyLayer;

    private bool isFrozen = false;
    private float freezeTimer = 0f;
    private float freezeSlowMultiplier = 1f;

    private bool isBurning = false;
    private float burnTimer = 0f;
    private float burnTickDamage = 0f;
    private float burnTickInterval = 0.4f;
    private float burnTickTimer = 0f;
    private float burnFlashTimer = 0f;
    private bool burnFlashYellow = false;

    private bool hasStoredOriginalColor = false;
    private Color originalColor = Color.white;

    private readonly Color freezeColor = new Color(0.3f, 0.7f, 1f, 1f);
    private readonly Color burnColor = new Color(1f, 0.55f, 0.05f, 1f);

    private float hitFlashTimer = 0f;
    private readonly Color hitColor = new Color(1f, 0.2f, 0.2f, 1f);

    public EnemyController(
        Transform enemyTransform,
        SpriteRenderer spriteRenderer,
        EnemyHealthSystem healthSystem
    )
    {
        SetCachedReferences(enemyTransform, spriteRenderer, healthSystem);
        StoreOriginalColorIfNeeded();
    }

    public void SetCachedReferences(
        Transform newEnemyTransform,
        SpriteRenderer newSpriteRenderer,
        EnemyHealthSystem newHealthSystem
    )
    {
        enemyTransform = newEnemyTransform;
        spriteRenderer = newSpriteRenderer;
        cachedHealth = newHealthSystem;

        StoreOriginalColorIfNeeded();
    }

    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
    }

    public void SetPlayerContact(float radius, float cooldown)
    {
        playerContactRadius = Mathf.Max(0.01f, radius);
        playerHitCooldown = Mathf.Max(0.01f, cooldown);
    }

    public void SetSeparation(float radius, float force, LayerMask layer)
    {
        separationRadius = radius;
        separationForce = force;
        enemyLayer = layer;
    }

    public void ResetState()
    {
        if (playerTransform == null)
            PlayerReferenceService.TryGetPlayer(out playerTransform);

        knockbackForce = 0f;
        knockbackVelocity = Vector2.zero;

        playerHitTimer = 0f;

        isFrozen = false;
        freezeTimer = 0f;
        freezeSlowMultiplier = 1f;

        isBurning = false;
        burnTimer = 0f;
        burnTickDamage = 0f;
        burnTickInterval = 0.4f;
        burnTickTimer = 0f;
        burnFlashTimer = 0f;
        burnFlashYellow = false;

        hitFlashTimer = 0f;

        ResetVisualState();
    }

    public void ResetVisualState()
    {
        StoreOriginalColorIfNeeded();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.flipX = false;
        }
    }

    public void Tick(float dt)
    {
        if (enemyTransform == null)
            return;

        if (playerTransform == null)
            PlayerReferenceService.TryGetPlayer(out playerTransform);

        if (playerTransform == null)
            return;

        UpdateFreeze(dt);
        UpdateBurn(dt);
        UpdateHitFlash(dt);
        UpdateKnockback(dt);
        UpdatePlayerHitTimer(dt);

        Vector2 direction = (playerTransform.position - enemyTransform.position).normalized;
        Vector2 separation = GetSeparationDirection();

        Vector2 finalDirection = direction + separation * separationForce * 0.35f;

        if (finalDirection.sqrMagnitude > 0.001f)
            finalDirection.Normalize();
        else
            finalDirection = direction;

        float currentSpeed = baseSpeed;

        if (isFrozen)
            currentSpeed *= freezeSlowMultiplier;

        Vector3 move = (Vector3)(finalDirection * currentSpeed * dt);
        Vector3 knockbackMove = (Vector3)knockbackVelocity * dt;

        enemyTransform.position += move + knockbackMove;

        UpdateSpriteFlip();
        CheckPlayerContactByDistance();
    }

    public void ApplyRadialKnockback(Vector3 sourcePosition, float radius, float force)
    {
        if (enemyTransform == null)
            return;

        float sqrDistance = (enemyTransform.position - sourcePosition).sqrMagnitude;
        float sqrRadius = radius * radius;

        if (sqrDistance > sqrRadius)
            return;

        Vector2 dir = (enemyTransform.position - sourcePosition).normalized;
        knockbackVelocity = dir * force;
        knockbackForce = force;
    }

    public void ApplyFreeze(float duration, float slowMultiplier)
    {
        if (isFrozen)
            return;

        isFrozen = true;
        freezeTimer = duration;
        freezeSlowMultiplier = slowMultiplier;

        RefreshVisualState();
    }

    public void ApplyBurn(float duration, float tickDamage, float tickInterval)
    {
        isBurning = true;
        burnTimer = duration;
        burnTickDamage = tickDamage;
        burnTickInterval = tickInterval;
        burnTickTimer = tickInterval;
        burnFlashTimer = 0f;
        burnFlashYellow = true;

        RefreshVisualState();
    }

    public void ApplyBulletHitFeedback(Vector3 sourcePosition, float force)
    {
        if (enemyTransform == null)
            return;

        Vector2 dir = (enemyTransform.position - sourcePosition).normalized;

        if (dir.sqrMagnitude <= 0.0001f)
            dir = Random.insideUnitCircle.normalized;

        knockbackVelocity = dir * force;
        knockbackForce = force;

        hitFlashTimer = 0.12f;
        RefreshVisualState();
    }

    private void UpdatePlayerHitTimer(float dt)
    {
        if (playerHitTimer <= 0f)
            return;

        playerHitTimer -= dt;

        if (playerHitTimer < 0f)
            playerHitTimer = 0f;
    }

    private void CheckPlayerContactByDistance()
    {
        if (playerHitTimer > 0f)
            return;

        if (playerTransform == null || enemyTransform == null)
            return;

        Vector2 enemyPosition = enemyTransform.position;
        Vector2 playerPosition = playerTransform.position;

        float sqrDistance = (enemyPosition - playerPosition).sqrMagnitude;
        float sqrContactRadius = playerContactRadius * playerContactRadius;

        if (sqrDistance > sqrContactRadius)
            return;

        playerHitTimer = playerHitCooldown;

        EventBus.Publish(new PlayerHitEvent(playerTransform.position));
    }

    private void StoreOriginalColorIfNeeded()
    {
        if (hasStoredOriginalColor)
            return;

        if (spriteRenderer == null)
            return;

        originalColor = spriteRenderer.color;
        hasStoredOriginalColor = true;
    }

    private Vector2 GetSeparationDirection()
    {
        if (enemyLayer.value == 0)
            return Vector2.zero;

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            enemyTransform.position,
            separationRadius,
            separationHitsBuffer,
            enemyLayer
        );

        Vector2 separation = Vector2.zero;
        int validCount = 0;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = separationHitsBuffer[i];

            if (hit == null)
                continue;

            if (hit.transform == enemyTransform)
                continue;

            Vector2 away = (Vector2)(enemyTransform.position - hit.transform.position);
            float distance = away.magnitude;

            if (distance <= 0.01f)
                away = Random.insideUnitCircle.normalized;
            else
                away /= distance;

            separation += away;
            validCount++;
        }

        ClearUsedBufferSlots(hitCount);

        if (validCount > 0)
            separation /= validCount;

        return separation.normalized;
    }

    private void ClearUsedBufferSlots(int usedCount)
    {
        for (int i = 0; i < usedCount; i++)
            separationHitsBuffer[i] = null;
    }

    private void UpdateFreeze(float dt)
    {
        if (!isFrozen)
            return;

        freezeTimer -= dt;

        if (freezeTimer <= 0f)
        {
            isFrozen = false;
            freezeTimer = 0f;
            freezeSlowMultiplier = 1f;

            RefreshVisualState();
        }
    }

    private void UpdateBurn(float dt)
    {
        if (!isBurning)
            return;

        burnTimer -= dt;
        burnTickTimer -= dt;
        burnFlashTimer -= dt;

        if (burnFlashTimer <= 0f)
        {
            burnFlashYellow = !burnFlashYellow;
            burnFlashTimer = 0.12f;
            RefreshVisualState();
        }

        if (burnTickTimer <= 0f)
        {
            burnTickTimer = burnTickInterval;

            if (cachedHealth != null)
                cachedHealth.TakeDamage(burnTickDamage);
        }

        if (burnTimer <= 0f)
        {
            isBurning = false;
            burnTimer = 0f;
            burnTickDamage = 0f;
            burnFlashYellow = false;

            RefreshVisualState();
        }
    }

    private void UpdateHitFlash(float dt)
    {
        if (hitFlashTimer <= 0f)
            return;

        hitFlashTimer -= dt;

        if (hitFlashTimer <= 0f)
        {
            hitFlashTimer = 0f;
            RefreshVisualState();
        }
    }

    private void UpdateKnockback(float dt)
    {
        if (knockbackForce <= 0f)
            return;

        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, 8f * dt);
        knockbackForce = knockbackVelocity.magnitude;

        if (knockbackForce < 0.05f)
        {
            knockbackVelocity = Vector2.zero;
            knockbackForce = 0f;
        }
    }

    private void RefreshVisualState()
    {
        if (spriteRenderer == null)
            return;

        StoreOriginalColorIfNeeded();

        if (hitFlashTimer > 0f)
        {
            spriteRenderer.color = hitColor;
            return;
        }

        if (isFrozen)
        {
            spriteRenderer.color = freezeColor;
            return;
        }

        if (isBurning)
        {
            spriteRenderer.color = burnFlashYellow ? burnColor : originalColor;
            return;
        }

        spriteRenderer.color = originalColor;
    }

    private void UpdateSpriteFlip()
    {
        if (spriteRenderer == null || playerTransform == null || enemyTransform == null)
            return;

        float directionToPlayer = playerTransform.position.x - enemyTransform.position.x;

        if (Mathf.Abs(directionToPlayer) < 0.01f)
            return;

        spriteRenderer.flipX = directionToPlayer < 0f;
    }
}