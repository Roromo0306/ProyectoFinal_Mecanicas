using System.Collections.Generic;
using UnityEngine;

public class XPObjectPool : MonoBehaviour
{
    public static XPObjectPool Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

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

        return Instance.SpawnInternal(prefab, position, rotation);
    }

    public static void Release(GameObject instance)
    {
        if (instance == null)
            return;

        if (Instance == null)
        {
            Destroy(instance);
            return;
        }

        Instance.ReleaseInternal(instance);
    }

    private static void EnsureInstance()
    {
        if (Instance != null)
            return;

        GameObject poolObject = new GameObject("XPObjectPool");
        Instance = poolObject.AddComponent<XPObjectPool>();
    }

    private GameObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject instance = GetAvailableInstance(prefab);

        instance.transform.SetParent(null);
        instance.transform.SetPositionAndRotation(position, rotation);

        PooledXPObject pooledObject = instance.GetComponent<PooledXPObject>();

        if (pooledObject == null)
            pooledObject = instance.AddComponent<PooledXPObject>();

        pooledObject.SetPrefab(prefab);

        instance.SetActive(true);

        return instance;
    }

    private GameObject GetAvailableInstance(GameObject prefab)
    {
        Queue<GameObject> pool = pools[prefab];

        while (pool.Count > 0)
        {
            GameObject instance = pool.Dequeue();

            if (instance != null)
                return instance;
        }

        GameObject newInstance = Instantiate(prefab);
        newInstance.name = prefab.name + "_Pooled";

        PooledXPObject pooledObject = newInstance.GetComponent<PooledXPObject>();

        if (pooledObject == null)
            pooledObject = newInstance.AddComponent<PooledXPObject>();

        pooledObject.SetPrefab(prefab);

        return newInstance;
    }

    private void ReleaseInternal(GameObject instance)
    {
        PooledXPObject pooledObject = instance.GetComponent<PooledXPObject>();

        if (pooledObject == null || pooledObject.Prefab == null)
        {
            Destroy(instance);
            return;
        }

        GameObject prefab = pooledObject.Prefab;

        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        instance.SetActive(false);
        instance.transform.SetParent(transform);

        pools[prefab].Enqueue(instance);
    }
}

public class PooledXPObject : MonoBehaviour
{
    public GameObject Prefab { get; private set; }

    public void SetPrefab(GameObject prefab)
    {
        Prefab = prefab;
    }
}