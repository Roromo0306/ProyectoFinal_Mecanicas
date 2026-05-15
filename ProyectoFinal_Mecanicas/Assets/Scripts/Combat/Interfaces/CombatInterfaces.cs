using UnityEngine;

public interface IDamageable
{
    GameObject TargetRoot { get; }
    void TakeDamage(float amount);
}

public interface IFreezable
{
    void ApplyFreeze(float duration, float slowMultiplier);
}

public interface IBurnable
{
    void ApplyBurn(float duration, float tickDamage, float tickInterval);
}

public interface IHitFeedbackReceiver
{
    void ApplyBulletHitFeedback(Vector3 sourcePosition, float force);
}