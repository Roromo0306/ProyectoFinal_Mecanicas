using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null)
            Debug.LogError("BulletSpawner -> No se encontró PlayerStats");
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ShootEvent>(OnShoot);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ShootEvent>(OnShoot);
    }

    private void OnShoot(object evt)
    {
        var e = (ShootEvent)evt;

        if (bulletPrefab == null) return;

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null) return;

        Vector3 baseDirection = e.direction.sqrMagnitude <= 0.0001f
            ? Vector3.right
            : e.direction.normalized;

        // Sonido una sola vez por disparo, aunque tengas Spread Shot.
        SFXManager.Instance?.PlayShoot();

        FireBullet(e.position, baseDirection);

        if (playerStats.hasSpreadShot)
        {
            FireBullet(e.position, RotateDirection(baseDirection, -playerStats.spreadAngle));
            FireBullet(e.position, RotateDirection(baseDirection, playerStats.spreadAngle));
        }
    }

    private void FireBullet(Vector3 position, Vector3 direction)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, position, Quaternion.identity);

        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if (bullet == null)
        {
            Destroy(bulletObj);
            return;
        }

        bullet.Init(
            direction,
            playerStats.damage,
            playerStats.pierceCount,
            playerStats.bounceCount,
            playerStats.bounceSearchRadius,
            playerStats.hasExplodingBullets,
            playerStats.explosionRadius,
            playerStats.explosionDamageMultiplier,
            playerStats.hasFreezeBullets,
            playerStats.freezeDuration,
            playerStats.freezeSlowMultiplier,
            playerStats.hasBurnBullets,
            playerStats.burnDuration,
            playerStats.burnTickDamage,
            playerStats.burnTickInterval
        );
    }

    private Vector3 RotateDirection(Vector3 direction, float angleDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angleDegrees);
        return (rotation * direction.normalized).normalized;
    }
}