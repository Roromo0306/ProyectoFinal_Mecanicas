using UnityEngine;

public class EliteEnemyController : MonoBehaviour
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

    private Transform player;
    private Camera mainCamera;

    private Vector3 enterTarget;
    private Vector3 chargeStartPosition;
    private Vector3 chargeDirection;
    private Vector3 repositionTarget;

    private float stateTimer;
    private float lastHitTime = -999f;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        mainCamera = Camera.main;
        enterTarget = GetPointInsideCamera();
    }

    private void Update()
    {
        if (player == null) return;

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
        transform.position = Vector3.MoveTowards(transform.position, enterTarget, enterSpeed * Time.deltaTime);

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
        transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

        float traveled = Vector3.Distance(transform.position, chargeStartPosition);

        if (traveled >= maxChargeDistance)
        {
            PickRepositionTarget();
            currentState = EliteState.Repositioning;
        }
    }

    private void UpdateRepositioning()
    {
        transform.position = Vector3.MoveTowards(transform.position, repositionTarget, repositionSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, repositionTarget) < 0.1f)
        {
            currentState = EliteState.Waiting;
            stateTimer = waitBeforeCharge;
        }
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
            return transform.position;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0.2f, 0.2f, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(0.8f, 0.8f, 0));

        return new Vector3(
            Random.Range(bottomLeft.x, topRight.x),
            Random.Range(bottomLeft.y, topRight.y),
            0f
        );
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Time.time - lastHitTime < contactDamageCooldown) return;

        lastHitTime = Time.time;
        EventBus.Publish(new PlayerHitEvent(collision.transform.position));
    }

    public void ApplyFreeze(float duration, float slowMultiplier)
    {
        // Versión simple: ralentiza temporalmente el movimiento del élite
        StartCoroutine(FreezeRoutine(duration, slowMultiplier));
    }

    private System.Collections.IEnumerator FreezeRoutine(float duration, float slowMultiplier)
    {
        float originalEnterSpeed = enterSpeed;
        float originalChargeSpeed = chargeSpeed;
        float originalRepositionSpeed = repositionSpeed;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color originalColor = Color.white;

        if (sr != null)
        {
            originalColor = sr.color;
            sr.color = Color.cyan;
        }

        enterSpeed *= slowMultiplier;
        chargeSpeed *= slowMultiplier;
        repositionSpeed *= slowMultiplier;

        yield return new WaitForSeconds(duration);

        enterSpeed = originalEnterSpeed;
        chargeSpeed = originalChargeSpeed;
        repositionSpeed = originalRepositionSpeed;

        if (sr != null)
            sr.color = originalColor;
    }

    public void ApplyBurn(float duration, float tickDamage, float tickInterval)
    {
        StartCoroutine(BurnRoutine(duration, tickDamage, tickInterval));
    }

    private System.Collections.IEnumerator BurnRoutine(float duration, float tickDamage, float tickInterval)
    {
        float timer = duration;
        float tickTimer = 0f;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Color originalColor = Color.white;

        if (sr != null)
            originalColor = sr.color;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            tickTimer -= Time.deltaTime;

            if (sr != null)
                sr.color = sr.color == Color.yellow ? originalColor : Color.yellow;

            if (tickTimer <= 0f)
            {
                tickTimer = tickInterval;

                EliteEnemyHealth health = GetComponent<EliteEnemyHealth>();
                if (health != null)
                    health.TakeDamage(tickDamage);
            }

            yield return new WaitForSeconds(0.12f);
        }

        if (sr != null)
            sr.color = originalColor;
    }
}