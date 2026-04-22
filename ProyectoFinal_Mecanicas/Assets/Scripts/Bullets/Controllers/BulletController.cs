using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    public GameObject explosionParticlePrefab;

    private Vector3 direction;
    private float damage;
    private int remainingHits;
    private int remainingBounces;
    private float bounceSearchRadius;

    private bool hasExplosion;
    private float explosionRadius;
    private float explosionDamageMultiplier;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void Init(
        Vector3 dir,
        float bulletDamage,
        int pierceCount,
        int bounceCount,
        float searchRadius,
        bool exploding,
        float explosionRadiusValue,
        float explosionDamageMultiplierValue
    )
    {
        direction = dir.normalized;
        damage = bulletDamage;

        remainingHits = Mathf.Max(pierceCount, bounceCount + 1);
        remainingBounces = bounceCount;
        bounceSearchRadius = searchRadius;

        hasExplosion = exploding;
        explosionRadius = explosionRadiusValue;
        explosionDamageMultiplier = explosionDamageMultiplierValue;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        GameObject target = collision.gameObject;

        if (hitEnemies.Contains(target))
            return;

        EnemyHealthSystem enemy = target.GetComponent<EnemyHealthSystem>();
        if (enemy == null)
            return;

        hitEnemies.Add(target);

        // daño al objetivo directo
        EventBus.Publish(new EnemyHitEvent(target, damage));

        // explosión
        if (hasExplosion)
        {
            Explode(target);
        }

        remainingHits--;

        // intentar rebote
        if (remainingBounces > 0)
        {
            GameObject nextTarget = FindNextEnemy(target);

            if (nextTarget != null)
            {
                remainingBounces--;

                Vector3 newDir = (nextTarget.transform.position - transform.position).normalized;
                direction = newDir;

                Debug.Log("Bounce hacia -> " + nextTarget.name + " | Bounces restantes: " + remainingBounces);
            }
        }

        if (remainingHits <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Explode(GameObject mainTarget)
    {
        if (explosionParticlePrefab != null)
        {
            GameObject fx = Instantiate(
                explosionParticlePrefab,
                mainTarget.transform.position,
                Quaternion.identity
            );

            // opcional: escalar según radio
            float scale = explosionRadius;
            fx.transform.localScale = Vector3.one * scale;
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(mainTarget.transform.position, explosionRadius);

        float explosionDamage = damage * explosionDamageMultiplier;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject candidate = hit.gameObject;

            if (candidate == mainTarget) continue;

            EnemyHealthSystem enemy = candidate.GetComponent<EnemyHealthSystem>();
            if (enemy == null) continue;

            EventBus.Publish(new EnemyHitEvent(candidate, explosionDamage));
        }

        Debug.Log("Explosión en " + mainTarget.name + " | Radio: " + explosionRadius);
    }

    private GameObject FindNextEnemy(GameObject currentTarget)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bounceSearchRadius);

        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject candidate = hit.gameObject;

            if (candidate == currentTarget) continue;
            if (hitEnemies.Contains(candidate)) continue;

            EnemyHealthSystem enemy = candidate.GetComponent<EnemyHealthSystem>();
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, candidate.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = candidate;
            }
        }

        return closestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bounceSearchRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}