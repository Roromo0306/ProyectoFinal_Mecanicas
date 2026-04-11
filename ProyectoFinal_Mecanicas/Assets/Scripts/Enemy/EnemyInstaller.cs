using UnityEngine;

public class EnemyInstaller : MonoBehaviour
{
    private EnemyController controller;

    private void Awake()
    {
        controller = new EnemyController(transform);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHitEvent>(OnPlayerHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHitEvent>(OnPlayerHit);
    }

    private void Update()
    {
        controller.Tick();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            controller.OnPlayerCollision(collision.transform);
        }
    }

    private void OnPlayerHit(object evt)
    {
        PlayerHitEvent hit = (PlayerHitEvent)evt;

        controller.ApplyRadialKnockback(
            hit.hitPosition,
            3f,
            20f
        );
    }
}