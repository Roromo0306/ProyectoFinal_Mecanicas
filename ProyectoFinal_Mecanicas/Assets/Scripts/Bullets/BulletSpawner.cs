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
            Debug.LogError("BulletSpawner -> playerStats es null");
            return;
        }

        var bulletObj = Instantiate(bulletPrefab, e.position, Quaternion.identity);

        BulletController bullet = bulletObj.GetComponent<BulletController>();
        if (bullet == null)
        {
            Debug.LogError("La bala no tiene BulletController");
            return;
        }

        bullet.Init(e.direction, playerStats.damage, playerStats.pierceCount);
    }
}