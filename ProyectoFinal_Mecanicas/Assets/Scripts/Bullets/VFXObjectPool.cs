using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXObjectPool : MonoBehaviour
{
    public static VFXObjectPool Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private readonly Dictionary<GameObject, Coroutine> releaseCoroutines = new Dictionary<GameObject, Coroutine>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {
        return Spawn(prefab, position, rotation, Vector3.one, lifetime);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, float lifetime)
    {
        if (prefab == null)
            return null;

        EnsureInstance();

        return Instance.SpawnInternal(prefab, position, rotation, scale, lifetime);
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

        GameObject poolObject = new GameObject("VFXObjectPool");
        Instance = poolObject.AddComponent<VFXObjectPool>();
    }

    private GameObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, float lifetime)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject instance = GetAvailableInstance(prefab);

        if (releaseCoroutines.TryGetValue(instance, out Coroutine activeCoroutine))
        {
            StopCoroutine(activeCoroutine);
            releaseCoroutines.Remove(instance);
        }

        instance.transform.SetParent(null);
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.transform.localScale = scale;

        PooledVFXObject pooledObject = instance.GetComponent<PooledVFXObject>();

        if (pooledObject == null)
            pooledObject = instance.AddComponent<PooledVFXObject>();

        pooledObject.SetPrefab(prefab);

        instance.SetActive(true);
        RestartParticles(instance);

        if (lifetime > 0f)
        {
            Coroutine releaseCoroutine = StartCoroutine(ReleaseAfterDelay(instance, lifetime));
            releaseCoroutines[instance] = releaseCoroutine;
        }

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
        newInstance.name = prefab.name + "_PooledVFX";

        PooledVFXObject pooledObject = newInstance.GetComponent<PooledVFXObject>();

        if (pooledObject == null)
            pooledObject = newInstance.AddComponent<PooledVFXObject>();

        pooledObject.SetPrefab(prefab);

        return newInstance;
    }

    private IEnumerator ReleaseAfterDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);

        ReleaseInternal(instance);
    }

    private void ReleaseInternal(GameObject instance)
    {
        if (instance == null)
            return;

        if (releaseCoroutines.ContainsKey(instance))
            releaseCoroutines.Remove(instance);

        PooledVFXObject pooledObject = instance.GetComponent<PooledVFXObject>();

        if (pooledObject == null || pooledObject.Prefab == null)
        {
            Destroy(instance);
            return;
        }

        StopParticles(instance);

        GameObject prefab = pooledObject.Prefab;

        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        instance.SetActive(false);
        instance.transform.SetParent(transform);

        pools[prefab].Enqueue(instance);
    }

    private void RestartParticles(GameObject instance)
    {
        ParticleSystem[] particles = instance.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play(true);
        }
    }

    private void StopParticles(GameObject instance)
    {
        ParticleSystem[] particles = instance.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}

public class PooledVFXObject : MonoBehaviour
{
    public GameObject Prefab { get; private set; }

    public void SetPrefab(GameObject prefab)
    {
        Prefab = prefab;
    }
}