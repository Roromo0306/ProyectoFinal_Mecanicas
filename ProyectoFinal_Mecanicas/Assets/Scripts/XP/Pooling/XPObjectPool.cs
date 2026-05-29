using UnityEngine;

public class XPObjectPool : MonoBehaviour
{
    public static XPObjectPool Instance { get; private set; }

    private const string PooledNameSuffix = "_PooledXP";

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

        return PoolService<PooledXPObject>.Spawn(prefab, position, rotation, PooledNameSuffix);
    }

    public static void Release(GameObject instance)
    {
        if (instance == null)
            return;

        PoolService<PooledXPObject>.Release(instance);
    }

    private static void EnsureInstance()
    {
        if (Instance != null)
            return;

        GameObject poolObject = new GameObject("XPObjectPool");
        Instance = poolObject.AddComponent<XPObjectPool>();
    }
}

public class PooledXPObject : PooledObject
{
}