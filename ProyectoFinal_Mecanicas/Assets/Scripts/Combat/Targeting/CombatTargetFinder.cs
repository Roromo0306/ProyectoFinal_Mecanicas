using UnityEngine;

public static class CombatTargetFinder
{
    public static GameObject GetDamageableRoot(GameObject source)
    {
        if (source == null)
            return null;

        CombatTargetCache cache = source.GetComponentInParent<CombatTargetCache>();

        if (cache != null && cache.Damageable != null)
            return cache.TargetRoot;

        if (TryGetInParent(source, out IDamageable damageable))
        {
            if (damageable.TargetRoot != null)
                return damageable.TargetRoot;

            MonoBehaviour behaviour = damageable as MonoBehaviour;

            if (behaviour != null)
                return behaviour.gameObject;
        }

        return null;
    }

    public static bool TryGetInParent<T>(GameObject source, out T result) where T : class
    {
        result = null;

        if (source == null)
            return false;

        CombatTargetCache cache = source.GetComponentInParent<CombatTargetCache>();

        if (cache != null && cache.TryGet(out result))
            return true;

        MonoBehaviour[] behaviours = source.GetComponentsInParent<MonoBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is T found)
            {
                result = found;
                return true;
            }
        }

        return false;
    }

    public static bool TryGetOnRoot<T>(GameObject root, out T result) where T : class
    {
        result = null;

        if (root == null)
            return false;

        CombatTargetCache cache = root.GetComponent<CombatTargetCache>();

        if (cache != null && cache.TryGet(out result))
            return true;

        MonoBehaviour[] behaviours = root.GetComponents<MonoBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is T found)
            {
                result = found;
                return true;
            }
        }

        return false;
    }
}