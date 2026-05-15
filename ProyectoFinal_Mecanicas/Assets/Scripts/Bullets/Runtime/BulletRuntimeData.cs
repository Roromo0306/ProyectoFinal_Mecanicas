using UnityEngine;

public struct BulletRuntimeData
{
    public Vector3 direction;
    public float damage;

    public int pierceCount;
    public int bounceCount;
    public float bounceSearchRadius;

    public bool hasExplosion;
    public float explosionRadius;
    public float explosionDamageMultiplier;

    public bool hasFreeze;
    public float freezeDuration;
    public float freezeSlowMultiplier;

    public bool hasBurn;
    public float burnDuration;
    public float burnTickDamage;
    public float burnTickInterval;

    public static BulletRuntimeData FromPlayerStats(Vector3 direction, float bulletDamage, PlayerStats stats)
    {
        BulletRuntimeData data = new BulletRuntimeData();

        data.direction = NormalizeDirection(direction);
        data.damage = bulletDamage;

        if (stats == null)
        {
            data.pierceCount = 1;
            data.bounceCount = 0;
            data.bounceSearchRadius = 0f;
            return data;
        }

        data.pierceCount = stats.pierceCount;
        data.bounceCount = stats.bounceCount;
        data.bounceSearchRadius = stats.bounceSearchRadius;

        data.hasExplosion = stats.hasExplodingBullets;
        data.explosionRadius = stats.explosionRadius;
        data.explosionDamageMultiplier = stats.explosionDamageMultiplier;

        data.hasFreeze = stats.hasFreezeBullets;
        data.freezeDuration = stats.freezeDuration;
        data.freezeSlowMultiplier = stats.freezeSlowMultiplier;

        data.hasBurn = stats.hasBurnBullets;
        data.burnDuration = stats.burnDuration;
        data.burnTickDamage = stats.burnTickDamage;
        data.burnTickInterval = stats.burnTickInterval;

        return data;
    }

    public static Vector3 NormalizeDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return Vector3.right;

        return direction.normalized;
    }
}