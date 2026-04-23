using UnityEngine;


public class PlayerController
{
    private PlayerModel model;
    private PlayerView view;
    private IInputService input;
    private PlayerStats playerStats;
    private PlayerHealthSystem playerHealth;

    private bool isDashing = false;
    private float dashEndTime = 0f;
    private float lastDashTime = -999f;
    private Vector2 dashDirection = Vector2.zero;

    public PlayerController(PlayerModel model, PlayerView view)
    {
        this.model = model;
        this.view = view;
        input = ServiceLocator.Get<IInputService>();

        playerStats = view.GetComponent<PlayerStats>();
        if (playerStats == null)
            Debug.LogError("PlayerController -> falta PlayerStats en " + view.gameObject.name);

        playerHealth = view.GetComponent<PlayerHealthSystem>();
        if (playerHealth == null)
            Debug.LogError("PlayerController -> falta PlayerHealthSystem en " + view.gameObject.name);

        if (view.dashTrail != null)
            view.dashTrail.emitting = false;
    }

    public void Tick()
    {
        HandleDash();
        Move();
        Shoot();
    }

    private void HandleDash()
    {
        if (playerStats == null || view == null || view.rb == null)
            return;

        // Terminar dash
        if (isDashing && Time.time >= dashEndTime)
        {
            isDashing = false;
            playerHealth?.SetExternalInvulnerable(false);

            if (view.spriteRenderer != null)
                view.spriteRenderer.color = Color.white;

            if (view.dashTrail != null)
                view.dashTrail.emitting = false;
        }

        if (!playerStats.hasDash)
            return;

        if (isDashing)
            return;

        if (!input.GetDash())
            return;

        if (Time.time - lastDashTime < playerStats.dashCooldown)
            return;

        Vector2 movementInput = input.GetMovement();

        if (movementInput.sqrMagnitude > 0.01f)
            dashDirection = movementInput.normalized;
        else
            dashDirection = view.transform.right;

        isDashing = true;
        dashEndTime = Time.time + playerStats.dashDuration;
        lastDashTime = Time.time;

        playerHealth?.SetExternalInvulnerable(true);

        if (view.spriteRenderer != null)
            view.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);

        if (view.dashTrail != null)
            view.dashTrail.emitting = true;

        CameraShakeService.Instance?.Shake(0.08f, 0.08f);
    }

    private void Move()
    {
        if (view == null || view.rb == null)
            return;

        if (isDashing)
        {
            view.rb.velocity = dashDirection * playerStats.dashSpeed;
            return;
        }

        Vector2 movement = input.GetMovement();

        float currentSpeed = model.speed;
        if (playerStats != null)
            currentSpeed = playerStats.moveSpeed;

        view.rb.velocity = movement.normalized * currentSpeed;
    }

    private void Shoot()
    {
        if (view == null)
        {
            Debug.LogError("PlayerController.Shoot -> view es null");
            return;
        }

        if (view.firePoint == null)
        {
            Debug.LogError("PlayerController.Shoot -> firePoint es null");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("PlayerController.Shoot -> playerStats es null");
            return;
        }

        if (!input.GetFire())
            return;

        if (Time.time - model.lastShootTime < playerStats.fireCooldown)
            return;

        model.lastShootTime = Time.time;

        Vector3 mousePos = input.GetMouseWorldPosition();
        Vector3 dir = (mousePos - view.firePoint.position).normalized;

        EventBus.Publish(new ShootEvent(view.firePoint.position, dir));
    }
}