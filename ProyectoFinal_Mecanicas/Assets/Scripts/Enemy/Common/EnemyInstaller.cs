using UnityEngine;

public class EnemyInstaller : MonoBehaviour, IFreezable, IBurnable, IHitFeedbackReceiver
{
    [Header("Enemy Stats")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.6f;
    [SerializeField] private float separationForce = 2.5f;
    [SerializeField] private LayerMask enemyLayer;

    private EnemyController controller;

    private void Awake()
    {
        CreateControllerIfNeeded();
    }

    private void OnEnable()
    {
        CreateControllerIfNeeded();

        controller.SetMoveSpeed(moveSpeed);
        controller.SetSeparation(separationRadius, separationForce, enemyLayer);
        controller.ResetState();

        EventBus.Subscribe<PlayerHitEvent>(OnPlayerHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHitEvent>(OnPlayerHit);

        if (controller != null)
            controller.ResetVisualState();
    }

    private void Update()
    {
        controller?.Tick(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            controller?.OnPlayerCollision(collision.transform);
    }

    private void OnPlayerHit(object evt)
    {
        PlayerHitEvent hit = (PlayerHitEvent)evt;

        controller?.ApplyRadialKnockback(
            hit.hitPosition,
            3f,
            20f
        );
    }

    public void ApplyFreeze(float duration, float slowMultiplier)
    {
        controller?.ApplyFreeze(duration, slowMultiplier);
    }

    public void ApplyBurn(float duration, float tickDamage, float tickInterval)
    {
        controller?.ApplyBurn(duration, tickDamage, tickInterval);
    }

    public void ApplyBulletHitFeedback(Vector3 sourcePosition, float force)
    {
        controller?.ApplyBulletHitFeedback(sourcePosition, force);
    }

    private void CreateControllerIfNeeded()
    {
        if (controller != null)
            return;

        controller = new EnemyController(transform);
    }
}