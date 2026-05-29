using System.Collections;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour, IDamageable, IHealthStatusProvider
{
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] private float deathDelay = 0.08f;

    private float currentHealth;
    private bool isDead = false;
    private Coroutine dieRoutine;
    private WaitForSeconds cachedDeathWait;

    private SpriteRenderer cachedSpriteRenderer;
    private EnemyXPDropper cachedXPDropper;

    public GameObject TargetRoot => gameObject;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        CacheReferences();
        cachedDeathWait = new WaitForSeconds(deathDelay);
        ResetHealth();
    }

    private void OnEnable()
    {
        CacheReferences();
        ResetHealth();
    }

    private void OnDisable()
    {
        if (dieRoutine != null)
        {
            StopCoroutine(dieRoutine);
            dieRoutine = null;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;

            if (dieRoutine == null)
                dieRoutine = StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        if (cachedSpriteRenderer != null)
        {
            cachedSpriteRenderer.color = Color.white;
            yield return cachedDeathWait;
        }

        if (cachedXPDropper != null)
            cachedXPDropper.DropXP();

        EventBus.Publish(new EnemyKilledEvent(
            gameObject,
            EnemyKillType.Normal,
            transform.position
        ));

        dieRoutine = null;
        EnemyObjectPool.Release(gameObject);
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        dieRoutine = null;
    }

    private void CacheReferences()
    {
        if (cachedSpriteRenderer == null)
            cachedSpriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (cachedXPDropper == null)
            TryGetComponent(out cachedXPDropper);
    }
}