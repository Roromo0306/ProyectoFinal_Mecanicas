using System.Collections;
using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] private float deathDelay = 0.08f;

    private float currentHealth;
    private bool isDead = false;
    private Coroutine dieRoutine;
    private WaitForSeconds cachedDeathWait;

    public GameObject TargetRoot => gameObject;

    private void Awake()
    {
        cachedDeathWait = new WaitForSeconds(deathDelay);
        ResetHealth();
    }

    private void OnEnable()
    {
        ResetHealth();
        EventBus.Subscribe<EnemyHitEvent>(OnHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyHitEvent>(OnHit);

        if (dieRoutine != null)
        {
            StopCoroutine(dieRoutine);
            dieRoutine = null;
        }
    }

    private void OnHit(EnemyHitEvent e)
    {
        if (e.enemy != gameObject)
            return;

        TakeDamage(e.damage);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            if (dieRoutine == null)
                dieRoutine = StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
            yield return cachedDeathWait;
        }

        EnemyXPDropper dropper = GetComponent<EnemyXPDropper>();
        if (dropper != null)
            dropper.DropXP();

        dieRoutine = null;
        EnemyObjectPool.Release(gameObject);
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        dieRoutine = null;
    }
}