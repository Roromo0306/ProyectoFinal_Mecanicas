using System.Collections.Generic;
using UnityEngine;

public static class PoolService<TPoolMarker> where TPoolMarker : PooledObject
{
    private static readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private static PoolServiceHost host;

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, string pooledNameSuffix = "_Pooled")
    {
        if (prefab == null)
            return null;

        EnsurePoolExists(prefab);

        GameObject instance = GetAvailableInstance(prefab, pooledNameSuffix);

        instance.transform.SetParent(null);
        instance.transform.SetPositionAndRotation(position, rotation);

        ResetRigidbody(instance);

        TPoolMarker marker = EnsurePoolMarker(instance);
        marker.SetPrefab(prefab);

        instance.SetActive(true);

        NotifySpawned(instance);

        return instance;
    }

    public static void Release(GameObject instance)
    {
        if (instance == null)
            return;

        TPoolMarker marker = instance.GetComponent<TPoolMarker>();

        if (marker == null || marker.Prefab == null)
        {
            Object.Destroy(instance);
            return;
        }

        NotifyDespawned(instance);

        ResetRigidbody(instance);

        GameObject prefab = marker.Prefab;

        EnsurePoolExists(prefab);

        instance.SetActive(false);
        instance.transform.SetParent(GetHostTransform());

        pools[prefab].Enqueue(instance);
    }

    public static void Clear()
    {
        foreach (KeyValuePair<GameObject, Queue<GameObject>> pair in pools)
        {
            Queue<GameObject> pool = pair.Value;

            while (pool.Count > 0)
            {
                GameObject instance = pool.Dequeue();

                if (instance != null)
                    Object.Destroy(instance);
            }
        }

        pools.Clear();
    }

    private static void EnsurePoolExists(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();
    }

    private static GameObject GetAvailableInstance(GameObject prefab, string pooledNameSuffix)
    {
        Queue<GameObject> pool = pools[prefab];

        while (pool.Count > 0)
        {
            GameObject instance = pool.Dequeue();

            if (instance != null)
                return instance;
        }

        GameObject newInstance = Object.Instantiate(prefab);
        newInstance.name = prefab.name + pooledNameSuffix;

        TPoolMarker marker = EnsurePoolMarker(newInstance);
        marker.SetPrefab(prefab);

        return newInstance;
    }

    private static TPoolMarker EnsurePoolMarker(GameObject instance)
    {
        TPoolMarker marker = instance.GetComponent<TPoolMarker>();

        if (marker == null)
            marker = instance.AddComponent<TPoolMarker>();

        return marker;
    }

    private static void ResetRigidbody(GameObject instance)
    {
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private static void NotifySpawned(GameObject instance)
    {
        MonoBehaviour[] behaviours = instance.GetComponentsInChildren<MonoBehaviour>(true);

        for (int i = 0; i < behaviours.Length; i++)
        {
            IPoolable poolable = behaviours[i] as IPoolable;

            if (poolable != null)
                poolable.OnSpawned();
        }
    }

    private static void NotifyDespawned(GameObject instance)
    {
        MonoBehaviour[] behaviours = instance.GetComponentsInChildren<MonoBehaviour>(true);

        for (int i = 0; i < behaviours.Length; i++)
        {
            IPoolable poolable = behaviours[i] as IPoolable;

            if (poolable != null)
                poolable.OnDespawned();
        }
    }

    private static Transform GetHostTransform()
    {
        return EnsureHost().transform;
    }

    private static PoolServiceHost EnsureHost()
    {
        if (host != null)
            return host;

        string hostName = "PoolService_" + typeof(TPoolMarker).Name;

        GameObject hostObject = GameObject.Find(hostName);

        if (hostObject == null)
            hostObject = new GameObject(hostName);

        host = hostObject.GetComponent<PoolServiceHost>();

        if (host == null)
            host = hostObject.AddComponent<PoolServiceHost>();

        return host;
    }
}

public sealed class PoolServiceHost : MonoBehaviour
{
}