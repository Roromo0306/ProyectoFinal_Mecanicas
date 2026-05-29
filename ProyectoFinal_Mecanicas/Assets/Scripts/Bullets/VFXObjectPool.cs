using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXObjectPool : MonoBehaviour
{
    public static VFXObjectPool Instance { get; private set; }

    private const string PooledNameSuffix = "_PooledVFX";

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

        EnsureInstance();

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
        GameObject instance = PoolService<PooledVFXObject>.Spawn(prefab, position, rotation, PooledNameSuffix);

        if (instance == null)
            return null;

        if (releaseCoroutines.TryGetValue(instance, out Coroutine activeCoroutine))
        {
            StopCoroutine(activeCoroutine);
            releaseCoroutines.Remove(instance);
        }

        instance.transform.localScale = scale;

        RestartParticles(instance);

        if (lifetime > 0f)
        {
            Coroutine releaseCoroutine = StartCoroutine(ReleaseAfterDelay(instance, lifetime));
            releaseCoroutines[instance] = releaseCoroutine;
        }

        return instance;
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

        StopParticles(instance);

        PoolService<PooledVFXObject>.Release(instance);
    }

    private void RestartParticles(GameObject instance)
    {
        ParticleSystem[] particles = instance.GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles[i].Play(true);
        }
    }

    private void StopParticles(GameObject instance)
    {
        ParticleSystem[] particles = instance.GetComponentsInChildren<ParticleSystem>(true);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}

public class PooledVFXObject : PooledObject
{
}