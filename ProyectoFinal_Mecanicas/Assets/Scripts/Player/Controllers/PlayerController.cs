using UnityEngine;

public class PlayerController
{
    private PlayerModel model;
    private PlayerView view;
    private IInputService input;

    public PlayerController(PlayerModel model, PlayerView view)
    {
        this.model = model;
        this.view = view;
        input = ServiceLocator.Get<IInputService>();
    }

    public void Tick()
    {
        Move();
        Shoot();
    }

    private void Move()
    {
        Vector2 movement = input.GetMovement();
        view.rb.velocity = movement.normalized * model.speed;
    }

    private void Shoot()
    {
        if (!input.GetFire())
            return;

        if (Time.time < model.lastShootTime + model.shootCooldown)
            return;

        model.lastShootTime = Time.time;

        Vector3 mousePos = input.GetMouseWorldPosition();
        Vector3 dir = (mousePos - view.firePoint.position).normalized;

        EventBus.Publish(new ShootEvent(view.firePoint.position, dir));
    }
}