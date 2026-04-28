using System.Collections;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] private float deathDelay = 0.08f;

    private float currentHealth;
    private bool isDead = false;

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
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(deathDelay);
        }

        EnemyXPDropper dropper = GetComponent<EnemyXPDropper>();
        if (dropper != null)
            dropper.DropXP();

        Destroy(gameObject);
    }
}