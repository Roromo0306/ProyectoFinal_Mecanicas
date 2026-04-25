using UnityEngine;

public class FinalBossHealth : MonoBehaviour
{
    public float maxHealth = 200f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        Debug.Log("Boss recibe daño: " + amount + " | Vida: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("BOSS FINAL DERROTADO");

        Destroy(gameObject);
    }
}