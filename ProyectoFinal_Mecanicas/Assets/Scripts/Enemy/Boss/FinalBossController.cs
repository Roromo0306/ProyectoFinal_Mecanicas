using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    [Header("Orbit")]
    public float orbitDistance = 6f;
    public float orbitSpeed = 30f;

    [Header("Movement")]
    public float chaseDistance = 9f;
    public float moveSpeed = 3f;

    [Header("Attack Positioning")]
    public float maxAttackDistance = 7f;
    public float minAttackDistance = 4f;
    public bool isReadyToAttack = false;

    [Header("Freeze")]
    public float bossFreezeDurationMultiplier = 0.35f;
    public Color freezeColor = new Color(0.3f, 0.7f, 1f, 1f);

    [Header("Burn")]
    public float bossBurnDurationMultiplier = 0.5f;
    public Color burnColor = new Color(1f, 0.55f, 0.05f, 1f);
    public Color burnFlashColor = Color.yellow;

    [Header("Hit Feedback")]
    public Color hitColor = new Color(1f, 0.2f, 0.2f, 1f);
    public float hitFlashDuration = 0.08f;

    private float hitFlashTimer = 0f;

    private Transform player;
    private float currentAngle;

    private BossLaserAttack laserAttack;

    private bool isFrozen = false;
    private float freezeTimer = 0f;
    private float freezeSlowMultiplier = 1f;

    private bool isBurning = false;
    private float burnTimer = 0f;
    private float burnTickDamage = 0f;
    private float burnTickInterval = 0.4f;
    private float burnTickTimer = 0f;
    private float burnFlashTimer = 0f;
    private bool burnFlashToggle = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        laserAttack = GetComponent<BossLaserAttack>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        currentAngle = Random.Range(0f, 360f);
    }

    private void Update()
    {
        UpdateFreeze();
        UpdateBurn();
        UpdateHitFlash();

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (laserAttack != null && laserAttack.IsAttacking)
            return;

        isReadyToAttack = distanceToPlayer <= maxAttackDistance;

        if (distanceToPlayer > maxAttackDistance)
        {
            FollowPlayer();
            return;
        }

        if (distanceToPlayer < minAttackDistance)
        {
            MoveAwayFromPlayer();
            return;
        }

        OrbitPlayer();
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
    public void ApplyBulletHitFeedback(Vector3 sourcePosition, float force)
    {
        hitFlashTimer = hitFlashDuration;
        RefreshVisualState();
    }
    private void FollowPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * GetCurrentMoveSpeed() * Time.deltaTime;
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        transform.position += dir * GetCurrentMoveSpeed() * Time.deltaTime;
    }

    private void OrbitPlayer()
    {
        currentAngle += orbitSpeed * GetCurrentFreezeMultiplier() * Time.deltaTime;

        float radians = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians),
            0f
        ) * orbitDistance;

        Vector3 targetPos = player.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * 2f * GetCurrentFreezeMultiplier()
        );
    }

    private float GetCurrentMoveSpeed()
    {
        return moveSpeed * GetCurrentFreezeMultiplier();
    }

    private float GetCurrentFreezeMultiplier()
    {
        return isFrozen ? freezeSlowMultiplier : 1f;
    }

    public void ApplyFreeze(float duration, float slowMultiplier)
    {
        if (isFrozen)
            return;

        isFrozen = true;

        freezeTimer = duration * bossFreezeDurationMultiplier;
        freezeSlowMultiplier = slowMultiplier;

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

    public void ApplyBurn(float duration, float tickDamage, float tickInterval)
    {
        if (isBurning)
            return;

        isBurning = true;

        burnTimer = duration * bossBurnDurationMultiplier;
        burnTickDamage = tickDamage;
        burnTickInterval = tickInterval;
        burnTickTimer = tickInterval;
        burnFlashTimer = 0f;
        burnFlashToggle = true;

        RefreshVisualState();
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
            burnFlashToggle = !burnFlashToggle;
            burnFlashTimer = 0.12f;
            RefreshVisualState();
        }

        if (burnTickTimer <= 0f)
        {
            burnTickTimer = burnTickInterval;

            FinalBossHealth bossHealth = GetComponent<FinalBossHealth>();
            if (bossHealth != null)
                bossHealth.TakeDamage(burnTickDamage);
        }

        if (burnTimer <= 0f)
        {
            isBurning = false;
            burnTimer = 0f;
            burnTickDamage = 0f;
            burnFlashToggle = false;

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
            spriteRenderer.color = burnFlashToggle ? burnFlashColor : burnColor;
            return;
        }

        spriteRenderer.color = originalColor;
    }

    public bool CanUseLaserAttack()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer <= maxAttackDistance && distanceToPlayer >= minAttackDistance;
    }
}