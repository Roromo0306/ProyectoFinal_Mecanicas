using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;

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

        var bullet = Instantiate(bulletPrefab, e.position, Quaternion.identity);
        bullet.GetComponent<BulletController>().Init(e.direction);
    }
}
