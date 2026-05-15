using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

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
            return;

        RefreshPlayerStatsIfNeeded();

        if (playerStats == null)
            return;

        Vector3 baseDirection = BulletRuntimeData.NormalizeDirection(firePoint.right);

        FireSingle(baseDirection);

        if (playerStats.hasSpreadShot)
        {
            FireSingle(RotateDirection(baseDirection, -playerStats.spreadAngle));
            FireSingle(RotateDirection(baseDirection, playerStats.spreadAngle));
        }
    }

    private void FireSingle(Vector3 direction)
    {
        GameObject bulletObj = BulletObjectPool.Spawn(bulletPrefab, firePoint.position, Quaternion.identity);

        if (bulletObj == null)
            return;

        BulletController bullet = bulletObj.GetComponent<BulletController>();

        if (bullet == null)
        {
            BulletObjectPool.Release(bulletObj);
            return;
        }

        BulletRuntimeData runtimeData = BulletRuntimeData.FromPlayerStats(
            direction,
            playerStats.damage,
            playerStats
        );

        bullet.Init(runtimeData);
    }

    private void RefreshPlayerStatsIfNeeded()
    {
        if (playerStats != null)
            return;

        playerStats = FindObjectOfType<PlayerStats>();
    }

    private Vector3 RotateDirection(Vector3 direction, float angleDegrees)
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, angleDegrees);
        return (rotation * direction.normalized).normalized;
    }
}