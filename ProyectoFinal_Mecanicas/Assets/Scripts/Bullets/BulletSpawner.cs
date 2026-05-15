using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;

    [Header("Spread Shot Balance")]
    [Range(0f, 1f)]
    public float spreadBulletDamageMultiplier = 0.35f;

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
        ShootEvent shootEvent = (ShootEvent)evt;

        if (bulletPrefab == null)
            return;

        RefreshPlayerStatsIfNeeded();

        if (playerStats == null)
            return;

        Vector3 baseDirection = BulletRuntimeData.NormalizeDirection(shootEvent.direction);

        SFXManager.Instance?.PlayShoot();

        if (playerStats.hasSpreadShot)
        {
            FireSpreadShot(shootEvent.position, baseDirection);
            return;
        }

        FireBullet(shootEvent.position, baseDirection, playerStats.damage);
    }

    private void FireSpreadShot(Vector3 position, Vector3 baseDirection)
    {
        float spreadDamage = playerStats.damage * spreadBulletDamageMultiplier;

        FireBullet(position, baseDirection, spreadDamage);
        FireBullet(position, RotateDirection(baseDirection, -playerStats.spreadAngle), spreadDamage);
        FireBullet(position, RotateDirection(baseDirection, playerStats.spreadAngle), spreadDamage);
    }

    private void FireBullet(Vector3 position, Vector3 direction, float bulletDamage)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, position, Quaternion.identity);

        BulletController bullet = bulletObj.GetComponent<BulletController>();

        if (bullet == null)
        {
            Destroy(bulletObj);
            return;
        }

        BulletRuntimeData runtimeData = BulletRuntimeData.FromPlayerStats(
            direction,
            bulletDamage,
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