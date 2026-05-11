using UnityEngine;

public class EnemyController
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    private float baseSpeed = 2f;
    private float knockbackForce = 0f;
    private Vector2 knockbackVelocity;

    // Separation
    private float separationRadius = 0.6f;
    private float separationForce = 2.5f;
    private LayerMask enemyLayer;

    // Freeze
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    private float freezeSlowMultiplier = 1f;

    // Burn
    private bool isBurning = false;
    private float burnTimer = 0f;
    private float burnTickDamage = 0f;
    private float burnTickInterval = 0.4f;
    private float burnTickTimer = 0f;
    private float burnFlashTimer = 0f;
    private bool burnFlashYellow = false;

    private Color originalColor = Color.white;
    private Color freezeColor = new Color(0.3f, 0.7f, 1f, 1f);
    private Color burnColor = new Color(1f, 0.55f, 0.05f, 1f);

    private float hitFlashTimer = 0f;

    // Cambia este color si quieres otro flash.
    private Color hitColor = new Color(1f, 0.2f, 0.2f, 1f);

    public EnemyController(Transform enemyTransform)
    {
        this.enemyTransform = enemyTransform;

        if (enemyTransform != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;

            spriteRenderer = enemyTransform.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        }
    }

    public void SetMoveSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
    }

    public void SetSeparation(float radius, float force, LayerMask layer)
    {
        separationRadius = radius;
        separationForce = force;
        enemyLayer = layer;
    }

    public void Tick()
    {
        if (enemyTransform == null || playerTransform == null)
            return;

        UpdateFreeze();
        UpdateBurn();
        UpdateHitFlash();
        UpdateKnockback();

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

        Vector3 move = (Vector3)(finalDirection * currentSpeed * Time.deltaTime);
        Vector3 knockbackMove = (Vector3)knockbackVelocity * Time.deltaTime;

        enemyTransform.position += move + knockbackMove;

        UpdateSpriteFlip();
    }

    private Vector2 GetSeparationDirection()
    {
        if (enemyLayer.value == 0)
            return Vector2.zero;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            enemyTransform.position,
            separationRadius,
            enemyLayer
        );

        Vector2 separation = Vector2.zero;
        int count = 0;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            if (hit.transform == enemyTransform) continue;

            Vector2 away = (Vector2)(enemyTransform.position - hit.transform.position);
            float distance = away.magnitude;

            if (distance <= 0.01f)
                away = Random.insideUnitCircle.normalized;
            else
                away /= distance;

            separation += away;
            count++;
        }

        if (count > 0)
            separation /= count;

        return separation.normalized;
    }

    public void OnPlayerCollision(Transform player)
    {
        if (player == null) return;

        EventBus.Publish(new PlayerHitEvent(player.position));
    }

    public void ApplyRadialKnockback(Vector3 sourcePosition, float radius, float force)
    {
        if (enemyTransform == null) return;

        float distance = Vector3.Distance(enemyTransform.position, sourcePosition);

        if (distance > radius)
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

    private void UpdateFreeze()
    {
        if (!isFrozen)
            return;

        freezeTimer -= Time.deltaTime;

        if (freezeTimer <= 0f)
        {
            isFrozen = false;
            freezeTimer = 0f;
            freezeSlowMultiplier = 1f;

            RefreshVisualState();
        }
    }

    private void UpdateBurn()
    {
        if (!isBurning)
            return;

        burnTimer -= Time.deltaTime;
        burnTickTimer -= Time.deltaTime;
        burnFlashTimer -= Time.deltaTime;

        if (burnFlashTimer <= 0f)
        {
            burnFlashYellow = !burnFlashYellow;
            burnFlashTimer = 0.12f;
            RefreshVisualState();
        }

        if (burnTickTimer <= 0f)
        {
            burnTickTimer = burnTickInterval;

            EnemyHealthSystem health = enemyTransform.GetComponent<EnemyHealthSystem>();
            if (health != null)
                health.TakeDamage(burnTickDamage);
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

    private void RefreshVisualState()
    {
        if (spriteRenderer == null)
            return;

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

    private void UpdateKnockback()
    {
        if (knockbackForce <= 0f)
            return;

        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, 8f * Time.deltaTime);
        knockbackForce = knockbackVelocity.magnitude;

        if (knockbackForce < 0.05f)
        {
            knockbackVelocity = Vector2.zero;
            knockbackForce = 0f;
        }
    }

    public void ApplyBulletHitFeedback(Vector3 sourcePosition, float force)
    {
        if (enemyTransform == null) return;

        Vector2 dir = (enemyTransform.position - sourcePosition).normalized;
        knockbackVelocity = dir * force;
        knockbackForce = force;

        hitFlashTimer = 0.12f;
        RefreshVisualState();
    }

    private void UpdateHitFlash()
    {
        if (hitFlashTimer <= 0f)
            return;

        hitFlashTimer -= Time.deltaTime;

        if (hitFlashTimer <= 0f)
        {
            hitFlashTimer = 0f;
            RefreshVisualState();
        }
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