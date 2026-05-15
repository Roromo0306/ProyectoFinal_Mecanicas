using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private const float DefaultDamage = 1f;
    private const int DefaultPierceCount = 1;
    private const float DefaultFireCooldown = 0.6f;

    private const bool DefaultHasSpreadShot = false;
    private const float DefaultSpreadAngle = 20f;

    private const int DefaultBounceCount = 0;
    private const float DefaultBounceSearchRadius = 6f;

    private const bool DefaultHasExplodingBullets = false;
    private const float DefaultExplosionRadius = 2.5f;
    private const float DefaultExplosionDamageMultiplier = 1f;

    private const bool DefaultHasFreezeBullets = false;
    private const float DefaultFreezeDuration = 2f;
    private const float DefaultFreezeSlowMultiplier = 0.4f;

    private const bool DefaultHasBurnBullets = false;
    private const float DefaultBurnDuration = 3f;
    private const float DefaultBurnTickDamage = 0.2f;
    private const float DefaultBurnTickInterval = 0.4f;

    private const float DefaultMoveSpeed = 5f;
    private const int DefaultMaxLives = 3;
    private const float DefaultMagnetRadius = 2f;

    private const bool DefaultHasDash = false;
    private const float DefaultDashSpeed = 18f;
    private const float DefaultDashDuration = 0.18f;
    private const float DefaultDashCooldown = 1.2f;

    private const bool DefaultHasRadialWeapon = false;
    private const float DefaultRadialOrbitRadius = 1.8f;
    private const float DefaultRadialOrbitSpeed = 180f;
    private const float DefaultRadialDamageMultiplier = 1f;

    private const bool DefaultHasAttackPet = false;
    private const bool DefaultHasSupportPet = false;
    private const float DefaultPetOrbitRadius = 2.2f;
    private const float DefaultPetOrbitSpeed = 160f;

    [Header("Combat")]
    public float damage = DefaultDamage;
    public int pierceCount = DefaultPierceCount;
    public float fireCooldown = DefaultFireCooldown;

    [Header("Projectiles")]
    public bool hasSpreadShot = DefaultHasSpreadShot;
    public float spreadAngle = DefaultSpreadAngle;

    public int bounceCount = DefaultBounceCount;
    public float bounceSearchRadius = DefaultBounceSearchRadius;

    public bool hasExplodingBullets = DefaultHasExplodingBullets;
    public float explosionRadius = DefaultExplosionRadius;
    public float explosionDamageMultiplier = DefaultExplosionDamageMultiplier;

    public bool hasFreezeBullets = DefaultHasFreezeBullets;
    public float freezeDuration = DefaultFreezeDuration;
    public float freezeSlowMultiplier = DefaultFreezeSlowMultiplier;

    public bool hasBurnBullets = DefaultHasBurnBullets;
    public float burnDuration = DefaultBurnDuration;
    public float burnTickDamage = DefaultBurnTickDamage;
    public float burnTickInterval = DefaultBurnTickInterval;

    [Header("Movement")]
    public float moveSpeed = DefaultMoveSpeed;

    [Header("Survival")]
    public int maxLives = DefaultMaxLives;

    [Header("Pickup")]
    public float magnetRadius = DefaultMagnetRadius;

    [Header("Dash")]
    public bool hasDash = DefaultHasDash;
    public float dashSpeed = DefaultDashSpeed;
    public float dashDuration = DefaultDashDuration;
    public float dashCooldown = DefaultDashCooldown;

    [Header("Radial Weapon")]
    public bool hasRadialWeapon = DefaultHasRadialWeapon;
    public float radialOrbitRadius = DefaultRadialOrbitRadius;
    public float radialOrbitSpeed = DefaultRadialOrbitSpeed;
    public float radialDamageMultiplier = DefaultRadialDamageMultiplier;

    [Header("Pets")]
    public bool hasAttackPet = DefaultHasAttackPet;
    public bool hasSupportPet = DefaultHasSupportPet;
    public float petOrbitRadius = DefaultPetOrbitRadius;
    public float petOrbitSpeed = DefaultPetOrbitSpeed;

    public void ResetToBase()
    {
        damage = DefaultDamage;
        pierceCount = DefaultPierceCount;
        fireCooldown = DefaultFireCooldown;

        hasSpreadShot = DefaultHasSpreadShot;
        spreadAngle = DefaultSpreadAngle;

        bounceCount = DefaultBounceCount;
        bounceSearchRadius = DefaultBounceSearchRadius;

        hasExplodingBullets = DefaultHasExplodingBullets;
        explosionRadius = DefaultExplosionRadius;
        explosionDamageMultiplier = DefaultExplosionDamageMultiplier;

        hasFreezeBullets = DefaultHasFreezeBullets;
        freezeDuration = DefaultFreezeDuration;
        freezeSlowMultiplier = DefaultFreezeSlowMultiplier;

        hasBurnBullets = DefaultHasBurnBullets;
        burnDuration = DefaultBurnDuration;
        burnTickDamage = DefaultBurnTickDamage;
        burnTickInterval = DefaultBurnTickInterval;

        moveSpeed = DefaultMoveSpeed;
        maxLives = DefaultMaxLives;
        magnetRadius = DefaultMagnetRadius;

        hasDash = DefaultHasDash;
        dashSpeed = DefaultDashSpeed;
        dashDuration = DefaultDashDuration;
        dashCooldown = DefaultDashCooldown;

        hasRadialWeapon = DefaultHasRadialWeapon;
        radialOrbitRadius = DefaultRadialOrbitRadius;
        radialOrbitSpeed = DefaultRadialOrbitSpeed;
        radialDamageMultiplier = DefaultRadialDamageMultiplier;

        hasAttackPet = DefaultHasAttackPet;
        hasSupportPet = DefaultHasSupportPet;
        petOrbitRadius = DefaultPetOrbitRadius;
        petOrbitSpeed = DefaultPetOrbitSpeed;
    }

    public void AddDamage(float amount)
    {
        damage += amount;
    }

    public void AddPierce(int amount)
    {
        pierceCount += amount;
    }

    public void AddFireRate(float cooldownReduction, float minimumCooldown)
    {
        fireCooldown = Mathf.Max(minimumCooldown, fireCooldown - cooldownReduction);
    }

    public void EnableSpreadShot(float angle)
    {
        hasSpreadShot = true;
        spreadAngle = Mathf.Max(spreadAngle, angle);
    }

    public void AddBounce(int amount, float radius)
    {
        bounceCount += amount;
        bounceSearchRadius = Mathf.Max(bounceSearchRadius, radius);
    }

    public void EnableExplodingBullets(float radius, float multiplier)
    {
        hasExplodingBullets = true;
        explosionRadius = Mathf.Max(explosionRadius, radius);
        explosionDamageMultiplier += multiplier;
    }

    public void EnableFreezeBullets(float duration, float slowMultiplier)
    {
        hasFreezeBullets = true;
        freezeDuration = Mathf.Max(freezeDuration, duration);
        freezeSlowMultiplier = Mathf.Min(freezeSlowMultiplier, slowMultiplier);
    }

    public void EnableBurnBullets(float duration, float tickDamage, float tickInterval)
    {
        hasBurnBullets = true;
        burnDuration = Mathf.Max(burnDuration, duration);
        burnTickDamage += tickDamage;
        burnTickInterval = Mathf.Min(burnTickInterval, tickInterval);
    }

    public void AddMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void AddMaxLives(int amount)
    {
        maxLives += amount;
    }

    public void AddMagnetRadius(float amount)
    {
        magnetRadius += amount;
    }

    public void EnableDash(float speed, float duration, float cooldown)
    {
        hasDash = true;
        dashSpeed = Mathf.Max(dashSpeed, speed);
        dashDuration = Mathf.Max(dashDuration, duration);
        dashCooldown = Mathf.Min(dashCooldown, cooldown);
    }

    public void EnableRadialWeapon(float orbitRadius, float orbitSpeed, float multiplier)
    {
        hasRadialWeapon = true;
        radialOrbitRadius = Mathf.Max(radialOrbitRadius, orbitRadius);
        radialOrbitSpeed += orbitSpeed;
        radialDamageMultiplier += multiplier;
    }

    public void EnableAttackPet(float orbitRadius, float orbitSpeed)
    {
        hasAttackPet = true;
        petOrbitRadius = Mathf.Max(petOrbitRadius, orbitRadius);
        petOrbitSpeed += orbitSpeed;
    }

    public void EnableSupportPet(float orbitRadius, float orbitSpeed)
    {
        hasSupportPet = true;
        petOrbitRadius = Mathf.Max(petOrbitRadius, orbitRadius);
        petOrbitSpeed += orbitSpeed;
    }
}