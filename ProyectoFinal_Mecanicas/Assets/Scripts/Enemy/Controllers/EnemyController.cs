using UnityEngine;

public class EnemyController
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    private float baseSpeed = 2f;
    private float knockbackForce = 0f;
    private Vector2 knockbackVelocity;

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
    private Color freezeColor = new Color(0.3f, 0.7f, 1f);
    private Color burnColor = Color.yellow;

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

    public void Tick()
    {
        if (enemyTransform == null || playerTransform == null)
            return;

        UpdateFreeze();
        UpdateBurn();
        UpdateKnockback();

        Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;

        float currentSpeed = baseSpeed;
        if (isFrozen)
            currentSpeed *= freezeSlowMultiplier;

        Vector3 move = direction * currentSpeed * Time.deltaTime;
        Vector3 knockbackMove = (Vector3)knockbackVelocity * Time.deltaTime;

        enemyTransform.position += move + knockbackMove;
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
            {
                health.TakeDamage(burnTickDamage);
            }
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
}