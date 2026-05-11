using UnityEngine;

public class EliteEnemyHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;

    [HideInInspector] public EliteEnemySpawner spawner;

    private bool isDead = false;
    private bool defeatNotified = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        Debug.Log("Elite recibe daño: " + amount + " | vida: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        NotifyDefeated();

        PlayerHealthSystem playerHealth = FindObjectOfType<PlayerHealthSystem>();
        if (playerHealth != null)
            playerHealth.AddMaxHeartAndHeal(1);

        SFXManager.Instance?.PlayEliteEnemyDeath();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NotifyDefeated();
    }

    private void NotifyDefeated()
    {
        if (defeatNotified) return;
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
}