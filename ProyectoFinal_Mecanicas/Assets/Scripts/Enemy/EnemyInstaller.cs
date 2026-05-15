using UnityEngine;

public class EnemyInstaller : MonoBehaviour, IFreezable, IBurnable, IHitFeedbackReceiver
{
    [Header("Enemy Stats")]
    public float moveSpeed = 2f;

    [Header("Separation")]
    public float separationRadius = 0.6f;
    public float separationForce = 2.5f;
    public LayerMask enemyLayer;

    private EnemyController controller;

    private void Awake()
    {
        controller = new EnemyController(transform);

        controller.SetMoveSpeed(moveSpeed);
        controller.SetSeparation(separationRadius, separationForce, enemyLayer);
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

    public void ApplyFreeze(float duration, float slowMultiplier)
    {
        controller.ApplyFreeze(duration, slowMultiplier);
    }

    public void ApplyBurn(float duration, float tickDamage, float tickInterval)
    {
        controller.ApplyBurn(duration, tickDamage, tickInterval);
    }

    public void ApplyBulletHitFeedback(Vector3 sourcePosition, float force)
    {
        controller?.ApplyBulletHitFeedback(sourcePosition, force);
    }
}