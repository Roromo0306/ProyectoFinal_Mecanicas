using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyHitEvent>(OnHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyHitEvent>(OnHit);
    }

    private void OnHit(object evt)
    {
        var e = (EnemyHitEvent)evt;

        if (e.enemy != gameObject)
            return;

        TakeDamage(e.damage);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}