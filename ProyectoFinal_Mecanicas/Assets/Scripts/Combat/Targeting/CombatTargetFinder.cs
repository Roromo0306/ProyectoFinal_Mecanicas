using UnityEngine;

public static class CombatTargetFinder
{
    public static GameObject GetDamageableRoot(GameObject source)
    {
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

        MonoBehaviour[] behaviours = source.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is T found)
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

        MonoBehaviour[] behaviours = root.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is T found)
            {
                result = found;
                return true;
            }
        }

        return false;
    }
}