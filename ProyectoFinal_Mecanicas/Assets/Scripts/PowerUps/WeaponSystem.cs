using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    public void Fire()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("WeaponSystem -> falta bulletPrefab o firePoint");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("WeaponSystem -> playerStats es null");
            return;
        }

        Vector3 baseDirection = firePoint.right;

        Debug.Log("Fire -> Spread activo: " + playerStats.hasSpreadShot);

        if (playerStats.hasSpreadShot)
        {
            FireSpread(baseDirection);
        }
        else
        {
            FireSingle(baseDirection);
        }
    }

    private void FireSingle(Vector3 direction)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if (bullet == null)
        {
            Debug.LogError("El prefab de bala no tiene BulletController");
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

    private void FireSpread(Vector3 baseDirection)
    {
        float angle = playerStats.spreadAngle;

        Vector3 leftDirection = RotateDirection(baseDirection, -angle);
        Vector3 rightDirection = RotateDirection(baseDirection, angle);

        FireSingle(baseDirection);
        FireSingle(leftDirection);
        FireSingle(rightDirection);
    }

    private Vector3 RotateDirection(Vector3 direction, float angleDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angleDegrees);
        return rotation * direction;
    }
}