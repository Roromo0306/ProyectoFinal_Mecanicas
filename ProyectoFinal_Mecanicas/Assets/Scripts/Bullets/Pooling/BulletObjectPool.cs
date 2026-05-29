using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{
    public static BulletObjectPool Instance { get; private set; }

    private const string PooledNameSuffix = "_PooledBullet";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
            return null;

        EnsureInstance();

        return PoolService<PooledBulletObject>.Spawn(prefab, position, rotation, PooledNameSuffix);
    }

    public static void Release(GameObject instance)
    {
        if (instance == null)
            return;

        PoolService<PooledBulletObject>.Release(instance);
    }

    private static void EnsureInstance()
    {
        if (Instance != null)
            return;

        GameObject poolObject = new GameObject("BulletObjectPool");
        Instance = poolObject.AddComponent<BulletObjectPool>();
    }
}

public class PooledBulletObject : PooledObject
{
}