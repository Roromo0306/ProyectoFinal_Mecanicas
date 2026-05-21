using UnityEngine;

public class EliteEnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 30f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private float currentHealth;

    [HideInInspector] public EliteEnemySpawner spawner;

    private bool isDead = false;
    private bool defeatNotified = false;

    public GameObject TargetRoot => gameObject;

    private void Awake()
    {
        ResetHealth();
    }

    private void OnEnable()
    {
        ResetHealth();
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

        currentHealth -= amount;

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

        EventBus.Publish(new EnemyKilledEvent(
            gameObject,
            EnemyKillType.Elite,
            transform.position
        ));

        NotifyDefeated();

        PlayerHealthSystem playerHealth = FindObjectOfType<PlayerHealthSystem>();
        if (playerHealth != null)
            playerHealth.AddMaxHeartAndHeal(1);

        SFXManager.Instance?.PlayEliteEnemyDeath();

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
    }
}