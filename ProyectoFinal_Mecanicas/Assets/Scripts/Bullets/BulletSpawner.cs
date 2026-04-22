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

        if (bulletPrefab == null)
        {
            Debug.LogError("BulletSpawner -> bulletPrefab es null");
            return;
        }

        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("BulletSpawner -> playerStats sigue siendo null");
                return;
            }
        }

        if (playerStats.hasSpreadShot)
        {
            FireBullet(e.position, e.direction);
            FireBullet(e.position, RotateDirection(e.direction, -playerStats.spreadAngle));
            FireBullet(e.position, RotateDirection(e.direction, playerStats.spreadAngle));
        }
        else
        {
            FireBullet(e.position, e.direction);
        }
    }

    private void FireBullet(Vector3 position, Vector3 direction)
    {
        var bulletObj = Instantiate(bulletPrefab, position, Quaternion.identity);

        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if (bullet == null)
        {
            Debug.LogError("La bala no tiene BulletController");
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
            playerStats.explosionDamageMultiplier
        );
    }

    private Vector3 RotateDirection(Vector3 direction, float angleDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angleDegrees);
        return rotation * direction.normalized;
    }
}