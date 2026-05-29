using UnityEngine;

public static class CombatDamageService
{
    public static bool IsValidTarget(GameObject targetRoot)
    {
        if (targetRoot == null)
            return false;

        if (!targetRoot.activeInHierarchy)
            return false;

        if (CombatTargetFinder.TryGetOnRoot(targetRoot, out IHealthStatusProvider healthStatus))
        {
            if (healthStatus.IsDead)
                return false;
        }

        return true;
    }

    public static bool TryDamage(GameObject targetRoot, float damage)
    {
        if (!IsValidTarget(targetRoot))
            return false;

        if (!CombatTargetFinder.TryGetOnRoot(targetRoot, out IDamageable damageable))
            return false;

        damageable.TakeDamage(damage);
        return true;
    }

    public static bool TryApplyFreeze(GameObject targetRoot, float duration, float slowMultiplier)
    {
        if (!IsValidTarget(targetRoot))
            return false;

        if (!CombatTargetFinder.TryGetOnRoot(targetRoot, out IFreezable freezable))
            return false;

        freezable.ApplyFreeze(duration, slowMultiplier);
        return true;
    }

    public static bool TryApplyBurn(GameObject targetRoot, float duration, float tickDamage, float tickInterval)
    {
        if (!IsValidTarget(targetRoot))
            return false;

        if (!CombatTargetFinder.TryGetOnRoot(targetRoot, out IBurnable burnable))
            return false;

        burnable.ApplyBurn(duration, tickDamage, tickInterval);
        return true;
    }

    public static bool TryApplyHitFeedback(GameObject targetRoot, Vector3 sourcePosition, float force)
    {
        if (!IsValidTarget(targetRoot))
            return false;

        if (!CombatTargetFinder.TryGetOnRoot(targetRoot, out IHitFeedbackReceiver feedbackReceiver))
            return false;

        feedbackReceiver.ApplyBulletHitFeedback(sourcePosition, force);
        return true;
    }
}