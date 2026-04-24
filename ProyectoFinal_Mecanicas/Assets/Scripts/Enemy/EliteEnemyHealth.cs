using UnityEngine;

public class EliteEnemyHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;

    [HideInInspector] public EliteEnemySpawner spawner;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        Debug.Log("Elite recibe daño: " + amount + " | vida: " + currentHealth);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        PlayerHealthSystem playerHealth = FindObjectOfType<PlayerHealthSystem>();
        if (playerHealth != null)
            playerHealth.AddLives(1);

        if (spawner != null)
            spawner.OnEliteDefeated();

        Destroy(gameObject);
    }
}