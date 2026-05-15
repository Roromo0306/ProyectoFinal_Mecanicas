using System.Collections;
using UnityEngine;

public class EliteEnemyController : MonoBehaviour, IFreezable, IBurnable, IHitFeedbackReceiver
{
    public enum EliteState
    {
        Entering,
        Waiting,
        Charging,
        Repositioning
    }

    public EliteState currentState = EliteState.Entering;

    public float enterSpeed = 4f;
    public float waitBeforeCharge = 1.5f;
    public float chargeSpeed = 12f;
    public float maxChargeDistance = 8f;
    public float repositionSpeed = 3f;
    public float repositionDistance = 2.5f;
    public float contactDamageCooldown = 0.4f;

    [Header("Hit Feedback")]
    public float hitFlashDuration = 0.08f;
    public Color hitColor = Color.white;

    private Transform player;
    private Camera mainCamera;

    private Vector3 enterTarget;
    private Vector3 chargeStartPosition;
    private Vector3 chargeDirection;
    private Vector3 repositionTarget;

    private float stateTimer;
    private float lastHitTime = -999f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private bool hasStoredOriginalColor = false;

    private Coroutine hitFlashRoutine;
    private Coroutine freezeRoutine;
    private Coroutine burnRoutine;

    private bool isFrozen = false;
    private bool isBurning = false;
    private bool burnFlashYellow = false;

    private float freezeSlowMultiplier = 1f;

    private EliteEnemyHealth cachedHealth;
    private WaitForSeconds cachedHitFlashWait;
    private static readonly WaitForSeconds burnFlashWait = new WaitForSeconds(0.12f);

    private void Awake()
    {
        RefreshReferences();
        StoreOriginalColorIfNeeded();
        cachedHealth = GetComponent<EliteEnemyHealth>();
        cachedHitFlashWait = new WaitForSeconds(hitFlashDuration);
    }

    private void OnEnable()
    {
        ResetForSpawn();
    }

    private void OnDisable()
    {
        StopStatusCoroutines();
        ResetVisualState();
    }

    public void ResetForSpawn()
    {
        RefreshReferences();
        StopStatusCoroutines();

        currentState = EliteState.Entering;

        enterTarget = GetPointInsideCamera();
        chargeStartPosition = transform.position;
        chargeDirection = Vector3.zero;
        repositionTarget = transform.position;

        stateTimer = 0f;
        lastHitTime = -999f;

        isFrozen = false;
        isBurning = false;
        burnFlashYellow = false;
        freezeSlowMultiplier = 1f;

        ResetVisualState();
    }

    private void Update()
    {
        if (player == null)
            TryFindPlayer();

        if (player == null)
            return;

        UpdateSpriteFlip();

        switch (currentState)
        {
            case EliteState.Entering:
                UpdateEntering();
                break;

            case EliteState.Waiting:
                UpdateWaiting();
                break;

            case EliteState.Charging:
                UpdateCharging();
                break;

            case EliteState.Repositioning:
                UpdateRepositioning();
                break;
        }
    }

    private void UpdateEntering()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            enterTarget,
            GetSpeed(enterSpeed) * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, enterTarget) < 0.1f)
        {
            currentState = EliteState.Waiting;
            stateTimer = waitBeforeCharge;
        }
    }

    private void UpdateWaiting()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            chargeStartPosition = transform.position;
            chargeDirection = (player.position - transform.position).normalized;
            currentState = EliteState.Charging;
        }
    }

    private void UpdateCharging()
    {
        transform.position += chargeDirection * GetSpeed(chargeSpeed) * Time.deltaTime;

        float traveled = Vector3.Distance(transform.position, chargeStartPosition);

        if (traveled >= maxChargeDistance)
        {
            PickRepositionTarget();
            currentState = EliteState.Repositioning;
        }
    }

    private void UpdateRepositioning()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            repositionTarget,
            GetSpeed(repositionSpeed) * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, repositionTarget) < 0.1f)
        {
            currentState = EliteState.Waiting;
            stateTimer = waitBeforeCharge;
        }
    }

    private float GetSpeed(float baseSpeed)
    {
        if (isFrozen)
            return baseSpeed * freezeSlowMultiplier;

        return baseSpeed;
    }

    private void PickRepositionTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle.normalized * repositionDistance;
        repositionTarget = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        if (mainCamera != null)
        {
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0.05f, 0.05f, 0));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(0.95f, 0.95f, 0));

            repositionTarget.x = Mathf.Clamp(repositionTarget.x, bottomLeft.x, topRight.x);
            repositionTarget.y = Mathf.Clamp(repositionTarget.y, bottomLeft.y, topRight.y);
        }
    }

    private Vector3 GetPointInsideCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return transform.position;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0.2f, 0.2f, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(0.8f, 0.8f, 0));

        return new Vector3(
            Random.Range(bottomLeft.x, topRight.x),
            Random.Range(bottomLeft.y, topRight.y),
            0f
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamagePlayer(collision.transform);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamagePlayer(collision.transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.transform);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.transform);
    }

    private void TryDamagePlayer(Transform target)
    {
        if (target == null)
            return;

        if (!target.CompareTag("Player"))
            return;

        if (Time.time - lastHitTime < contactDamageCooldown)
            return;

        lastHitTime = Time.time;
        EventBus.Publish(new PlayerHitEvent(target.position));
    }

    public void ApplyBulletHitFeedback(Vector3 sourcePosition, float force)
    {
        if (hitFlashRoutine != null)
            StopCoroutine(hitFlashRoutine);

        hitFlashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = hitColor;

        yield return cachedHitFlashWait;

        hitFlashRoutine = null;
        RefreshVisualState();
    }

    public void ApplyFreeze(float duration, float slowMultiplier)
    {
        if (isFrozen || freezeRoutine != null)
            return;

        freezeRoutine = StartCoroutine(FreezeRoutine(duration, slowMultiplier));
    }

    private IEnumerator FreezeRoutine(float duration, float slowMultiplier)
    {
        isFrozen = true;
        freezeSlowMultiplier = slowMultiplier;

        RefreshVisualState();

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        freezeSlowMultiplier = 1f;
        freezeRoutine = null;

        RefreshVisualState();
    }

    public void ApplyBurn(float duration, float tickDamage, float tickInterval)
    {
        if (burnRoutine != null)
            StopCoroutine(burnRoutine);

        burnRoutine = StartCoroutine(BurnRoutine(duration, tickDamage, tickInterval));
    }

    private IEnumerator BurnRoutine(float duration, float tickDamage, float tickInterval)
    {
        isBurning = true;

        float timer = duration;
        float tickTimer = 0f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            tickTimer -= Time.deltaTime;

            burnFlashYellow = !burnFlashYellow;
            RefreshVisualState();

            if (tickTimer <= 0f)
            {
                tickTimer = tickInterval;

                if (cachedHealth != null)
                    cachedHealth.TakeDamage(tickDamage);
            }

            yield return burnFlashWait;
        }

        isBurning = false;
        burnFlashYellow = false;
        burnRoutine = null;

        RefreshVisualState();
    }

    private void RefreshReferences()
    {
        TryFindPlayer();

        if (mainCamera == null)
            mainCamera = Camera.main;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StoreOriginalColorIfNeeded();
    }

    private void TryFindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
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

    private void StopStatusCoroutines()
    {
        if (hitFlashRoutine != null)
        {
            StopCoroutine(hitFlashRoutine);
            hitFlashRoutine = null;
        }

        if (freezeRoutine != null)
        {
            StopCoroutine(freezeRoutine);
            freezeRoutine = null;
        }

        if (burnRoutine != null)
        {
            StopCoroutine(burnRoutine);
            burnRoutine = null;
        }
    }

    private void ResetVisualState()
    {
        RefreshReferences();

        isFrozen = false;
        isBurning = false;
        burnFlashYellow = false;
        freezeSlowMultiplier = 1f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.flipX = false;
        }
    }

    private void RefreshVisualState()
    {
        if (spriteRenderer == null)
            return;

        StoreOriginalColorIfNeeded();

        if (hitFlashRoutine != null)
        {
            spriteRenderer.color = hitColor;
            return;
        }

        if (isFrozen)
        {
            spriteRenderer.color = Color.cyan;
            return;
        }

        if (isBurning)
        {
            spriteRenderer.color = burnFlashYellow ? Color.yellow : originalColor;
            return;
        }

        spriteRenderer.color = originalColor;
    }

    private void UpdateSpriteFlip()
    {
        if (spriteRenderer == null || player == null)
            return;

        float directionToPlayer = player.position.x - transform.position.x;

        if (Mathf.Abs(directionToPlayer) < 0.01f)
            return;

        spriteRenderer.flipX = directionToPlayer < 0f;
    }
}