using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    private Vector3 direction;
    private float damage;
    private int remainingHits;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void Init(Vector3 dir, float bulletDamage, int pierceCount)
    {
        direction = dir.normalized;
        damage = bulletDamage;
        remainingHits = pierceCount;

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

        EventBus.Publish(new EnemyHitEvent(target, damage));

        remainingHits--;

        if (remainingHits <= 0)
        {
            Destroy(gameObject);
        }
    }
}