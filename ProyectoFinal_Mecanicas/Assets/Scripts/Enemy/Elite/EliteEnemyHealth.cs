using System.Collections;
using UnityEngine;

public class EliteEnemyHealth : MonoBehaviour, IDamageable, IHealthStatusProvider
{
    public float maxHealth = 30f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private float currentHealth;

    [HideInInspector] public EliteEnemySpawner spawner;

    private bool isDead = false;
    private bool defeatNotified = false;
    private Coroutine deathRoutine;

    public GameObject TargetRoot => gameObject;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        ResetHealth();
    }

    private void OnEnable()
    {
        ResetHealth();
    }

    private void OnDisable()
    {
        deathRoutine = null;
    }

    public void PrepareForSpawn(EliteEnemySpawner owner)
    {
        spawner = owner;
        ResetHealth();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        PublishHealthChanged();

        if (showDebugLogs)
            Debug.Log("Elite recibe daño: " + amount + " | vida: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        PublishHealthChanged();

        if (deathRoutine == null)
            deathRoutine = StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Vector3 deathPosition = transform.position;

        NotifyDefeated();

        PlayerHealthSystem playerHealth = FindObjectOfType<PlayerHealthSystem>();
        if (playerHealth != null)
            playerHealth.AddMaxHeartAndHeal(1);

        SFXManager.Instance?.PlayEliteEnemyDeath();

        try
        {
            EventBus.Publish(new EnemyKilledEvent(
                gameObject,
                EnemyKillType.Elite,
                deathPosition
            ));
        }
        catch (System.Exception e)
        {
            Debug.LogError("EliteEnemyHealth -> Error publicando EnemyKilledEvent:\n" + e);
        }

        // IMPORTANTE:
        // Esperamos un frame antes de devolverlo al pool.
        // Así evitamos que la bala, el deck o el level up sigan usando un enemigo ya desactivado.
        yield return null;

        deathRoutine = null;

        if (gameObject.activeInHierarchy)
            EnemyObjectPool.Release(gameObject);
    }

    private void NotifyDefeated()
    {
        if (defeatNotified)
            return;

        defeatNotified = true;

        if (spawner == null)
            spawner = FindObjectOfType<EliteEnemySpawner>();

        if (spawner != null)
        {
            spawner.OnEliteDefeated();
        }
        else
        {
            ArenaClosureController arena = FindObjectOfType<ArenaClosureController>();

            if (arena != null)
                arena.DeactivateArena();
        }
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        defeatNotified = false;
        deathRoutine = null;

        PublishHealthChanged();
    }

    private void PublishHealthChanged()
    {
        EventBus.Publish(new DamageableHealthChangedEvent(
            gameObject,
            currentHealth,
            maxHealth,
            isDead
        ));
    }
}