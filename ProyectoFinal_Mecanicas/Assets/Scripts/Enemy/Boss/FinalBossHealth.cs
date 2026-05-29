using UnityEngine;

public class FinalBossHealth : MonoBehaviour, IDamageable, IHealthStatusProvider
{
    public float maxHealth = 200f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private float currentHealth;
    private bool isDead = false;

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

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        PublishHealthChanged();

        if (showDebugLogs)
            Debug.Log("Boss recibe daño: " + amount + " | Vida: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        StopBossAttacks();

        PublishHealthChanged();

        if (showDebugLogs)
            Debug.Log("BOSS FINAL DERROTADO");

        Destroy(gameObject);
    }

    private void StopBossAttacks()
    {
        BossLaserAttack[] laserAttacks = GetComponentsInChildren<BossLaserAttack>(true);

        for (int i = 0; i < laserAttacks.Length; i++)
        {
            if (laserAttacks[i] != null)
                laserAttacks[i].StopAttackAndClearLasers();
        }
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

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