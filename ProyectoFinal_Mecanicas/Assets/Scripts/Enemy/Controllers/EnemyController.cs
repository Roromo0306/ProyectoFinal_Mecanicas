using UnityEngine;

public class EnemyController
{
    private Transform transform;
    private Transform player;

    public EnemyController(Transform transform)
    {
        this.transform = transform;
        player = GameObject.FindWithTag("Player").transform;
    }

    public void Tick()
    {
        var dir = (player.position - transform.position).normalized;
        transform.position += dir * 2f * Time.deltaTime;
    }

    public void OnPlayerCollision(Transform playerTransform)
    {
        EventBus.Publish(new PlayerHitEvent());

        var dir = (transform.position - playerTransform.position).normalized;
        transform.position += dir;
    }
}