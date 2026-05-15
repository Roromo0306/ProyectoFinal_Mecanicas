using UnityEngine;

public class FinalBossHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 200f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private float currentHealth;
    private bool isDead = false;

    public GameObject TargetRoot => gameObject;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        if (showDebugLogs)
            Debug.Log("Boss recibe daño: " + amount + " | Vida: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (showDebugLogs)
            Debug.Log("BOSS FINAL DERROTADO");

        Destroy(gameObject);
    }
}