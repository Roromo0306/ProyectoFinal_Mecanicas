using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    private Vector3 direction;

    public void Init(Vector3 dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventBus.Publish(new EnemyHitEvent(collision.gameObject));
        Destroy(gameObject);
    }
}
