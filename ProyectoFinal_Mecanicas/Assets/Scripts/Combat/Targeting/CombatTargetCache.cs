using UnityEngine;

public class CombatTargetCache : MonoBehaviour
{
    public GameObject TargetRoot { get; private set; }

    public IDamageable Damageable { get; private set; }
    public IHealthStatusProvider HealthStatus { get; private set; }
    public IFreezable Freezable { get; private set; }
    public IBurnable Burnable { get; private set; }
    public IHitFeedbackReceiver HitFeedbackReceiver { get; private set; }

    private bool referencesCached;

    private void Awake()
    {
        CacheReferences();
    }

    private void OnEnable()
    {
        CacheReferences();
    }

    public void CacheReferences()
    {
        TargetRoot = gameObject;

        Damageable = FindOnRoot<IDamageable>();
        HealthStatus = FindOnRoot<IHealthStatusProvider>();
        Freezable = FindOnRoot<IFreezable>();
        Burnable = FindOnRoot<IBurnable>();
        HitFeedbackReceiver = FindOnRoot<IHitFeedbackReceiver>();

        referencesCached = true;
    }

    public bool TryGet<T>(out T result) where T : class
    {
        if (!referencesCached)
            CacheReferences();

        result = null;

        if (typeof(T) == typeof(IDamageable))
            result = Damageable as T;
        else if (typeof(T) == typeof(IHealthStatusProvider))
            result = HealthStatus as T;
        else if (typeof(T) == typeof(IFreezable))
            result = Freezable as T;
        else if (typeof(T) == typeof(IBurnable))
            result = Burnable as T;
        else if (typeof(T) == typeof(IHitFeedbackReceiver))
            result = HitFeedbackReceiver as T;

        return result != null;
    }

    private T FindOnRoot<T>() where T : class
    {
        MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is T found)
                return found;
        }

        return null;
    }
}