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
        if (bulletPrefab == null || firePoint == null) return;

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null) return;

        Vector3 baseDirection = firePoint.right.sqrMagnitude <= 0.0001f
            ? Vector3.right
            : firePoint.right.normalized;

        FireSingle(baseDirection);

        if (playerStats.hasSpreadShot)
        {
            FireSingle(RotateDirection(baseDirection, -playerStats.spreadAngle));
            FireSingle(RotateDirection(baseDirection, playerStats.spreadAngle));
        }
    }

    private void FireSingle(Vector3 direction)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

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