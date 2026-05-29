using UnityEngine;

public class EnemyInstaller : MonoBehaviour, IFreezable, IBurnable, IHitFeedbackReceiver
{
    [Header("Enemy Stats")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Player Contact")]
    [SerializeField] private float playerContactRadius = 0.45f;
    [SerializeField] private float playerHitCooldown = 0.5f;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.6f;
    [SerializeField] private float separationForce = 2.5f;
    [SerializeField] private LayerMask enemyLayer;

    private EnemyController controller;

    private Transform cachedTransform;
    private SpriteRenderer cachedSpriteRenderer;
    private EnemyHealthSystem cachedHealthSystem;
    private CombatTargetCache cachedTargetCache;

    private void Awake()
    {
        CacheReferences();
        CreateControllerIfNeeded();
    }

    private void OnEnable()
    {
        CacheReferences();
        CreateControllerIfNeeded();

        controller.SetMoveSpeed(moveSpeed);
        controller.SetSeparation(separationRadius, separationForce, enemyLayer);
        controller.SetPlayerContact(playerContactRadius, playerHitCooldown);

        if (PlayerReferenceService.TryGetPlayer(out Transform player))
            controller.SetPlayer(player);

        controller.ResetState();

        EnemyUpdateManager.Register(this);

        EventBus.Subscribe<PlayerHitEvent>(OnPlayerHit);
    }

    private void OnDisable()
    {
        EnemyUpdateManager.Unregister(this);

        EventBus.Unsubscribe<PlayerHitEvent>(OnPlayerHit);

        if (controller != null)
            controller.ResetVisualState();
    }

    public void TickEnemy(float deltaTime)
    {
        controller?.Tick(deltaTime);
    }

    private void OnPlayerHit(PlayerHitEvent hit)
    {
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

    private void CacheReferences()
    {
        if (cachedTransform == null)
            cachedTransform = transform;

        if (cachedSpriteRenderer == null)
            cachedSpriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (cachedHealthSystem == null)
            TryGetComponent(out cachedHealthSystem);

        if (cachedTargetCache == null)
        {
            cachedTargetCache = GetComponent<CombatTargetCache>();

            if (cachedTargetCache == null)
                cachedTargetCache = gameObject.AddComponent<CombatTargetCache>();
        }

        cachedTargetCache.CacheReferences();
    }

    private void CreateControllerIfNeeded()
    {
        if (controller != null)
        {
            controller.SetCachedReferences(
                cachedTransform,
                cachedSpriteRenderer,
                cachedHealthSystem
            );

            return;
        }

        controller = new EnemyController(
            cachedTransform,
            cachedSpriteRenderer,
            cachedHealthSystem
        );
    }
}