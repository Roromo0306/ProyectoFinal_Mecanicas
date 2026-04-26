using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Combat")]
    public float damage = 1f;
    public int pierceCount = 1;
    public float fireCooldown = 0.3f;

    [Header("Projectiles")]
    public bool hasSpreadShot = false;
    public float spreadAngle = 20f;

    public int bounceCount = 0;
    public float bounceSearchRadius = 6f;

    public bool hasExplodingBullets = false;
    public float explosionRadius = 2.5f;
    public float explosionDamageMultiplier = 1f;

    public bool hasFreezeBullets = false;
    public float freezeDuration = 2f;
    public float freezeSlowMultiplier = 0.4f;

    public bool hasBurnBullets = false;
    public float burnDuration = 3f;
    public float burnTickDamage = 0.2f;
    public float burnTickInterval = 0.4f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Survival")]
    public int maxLives = 3;

    [Header("Pickup")]
    public float magnetRadius = 2f;

    [Header("Dash")]
    public bool hasDash = false;
    public float dashSpeed = 18f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 1.2f;

    [Header("Radial Weapon")]
    public bool hasRadialWeapon = false;
    public float radialOrbitRadius = 1.8f;
    public float radialOrbitSpeed = 180f;
    public float radialDamageMultiplier = 1f;

    [Header("Pets")]
    public bool hasAttackPet = false;
    public bool hasSupportPet = false;
    public float petOrbitRadius = 2.2f;
    public float petOrbitSpeed = 160f;

    private BaseStats baseStats;
    private bool baseSaved;

    private void Awake()
    {
        SaveBaseStats();
    }

    private void SaveBaseStats()
    {
        baseStats.damage = damage;
        baseStats.pierceCount = pierceCount;
        baseStats.fireCooldown = fireCooldown;

        baseStats.hasSpreadShot = hasSpreadShot;
        baseStats.spreadAngle = spreadAngle;

        baseStats.bounceCount = bounceCount;
        baseStats.bounceSearchRadius = bounceSearchRadius;

        baseStats.hasExplodingBullets = hasExplodingBullets;
        baseStats.explosionRadius = explosionRadius;
        baseStats.explosionDamageMultiplier = explosionDamageMultiplier;

        baseStats.hasFreezeBullets = hasFreezeBullets;
        baseStats.freezeDuration = freezeDuration;
        baseStats.freezeSlowMultiplier = freezeSlowMultiplier;

        baseStats.hasBurnBullets = hasBurnBullets;
        baseStats.burnDuration = burnDuration;
        baseStats.burnTickDamage = burnTickDamage;
        baseStats.burnTickInterval = burnTickInterval;

        baseStats.moveSpeed = moveSpeed;
        baseStats.maxLives = maxLives;
        baseStats.magnetRadius = magnetRadius;

        baseStats.hasDash = hasDash;
        baseStats.dashSpeed = dashSpeed;
        baseStats.dashDuration = dashDuration;
        baseStats.dashCooldown = dashCooldown;

        baseStats.hasRadialWeapon = hasRadialWeapon;
        baseStats.radialOrbitRadius = radialOrbitRadius;
        baseStats.radialOrbitSpeed = radialOrbitSpeed;
        baseStats.radialDamageMultiplier = radialDamageMultiplier;

        baseStats.hasAttackPet = hasAttackPet;
        baseStats.hasSupportPet = hasSupportPet;
        baseStats.petOrbitRadius = petOrbitRadius;
        baseStats.petOrbitSpeed = petOrbitSpeed;

        baseSaved = true;
    }

    public void ResetToBase()
    {
        if (!baseSaved)
            SaveBaseStats();

        damage = baseStats.damage;
        pierceCount = baseStats.pierceCount;
        fireCooldown = baseStats.fireCooldown;

        hasSpreadShot = baseStats.hasSpreadShot;
        spreadAngle = baseStats.spreadAngle;

        bounceCount = baseStats.bounceCount;
        bounceSearchRadius = baseStats.bounceSearchRadius;

        hasExplodingBullets = baseStats.hasExplodingBullets;
        explosionRadius = baseStats.explosionRadius;
        explosionDamageMultiplier = baseStats.explosionDamageMultiplier;

        hasFreezeBullets = baseStats.hasFreezeBullets;
        freezeDuration = baseStats.freezeDuration;
        freezeSlowMultiplier = baseStats.freezeSlowMultiplier;

        hasBurnBullets = baseStats.hasBurnBullets;
        burnDuration = baseStats.burnDuration;
        burnTickDamage = baseStats.burnTickDamage;
        burnTickInterval = baseStats.burnTickInterval;

        moveSpeed = baseStats.moveSpeed;
        maxLives = baseStats.maxLives;
        magnetRadius = baseStats.magnetRadius;

        hasDash = baseStats.hasDash;
        dashSpeed = baseStats.dashSpeed;
        dashDuration = baseStats.dashDuration;
        dashCooldown = baseStats.dashCooldown;

        hasRadialWeapon = baseStats.hasRadialWeapon;
        radialOrbitRadius = baseStats.radialOrbitRadius;
        radialOrbitSpeed = baseStats.radialOrbitSpeed;
        radialDamageMultiplier = baseStats.radialDamageMultiplier;

        hasAttackPet = baseStats.hasAttackPet;
        hasSupportPet = baseStats.hasSupportPet;
        petOrbitRadius = baseStats.petOrbitRadius;
        petOrbitSpeed = baseStats.petOrbitSpeed;
    }

    public void AddDamage(float amount) => damage += amount;
    public void AddPierce(int amount) => pierceCount += amount;

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

    public void AddMoveSpeed(float amount) => moveSpeed += amount;
    public void AddMaxLives(int amount) => maxLives += amount;
    public void AddMagnetRadius(float amount) => magnetRadius += amount;

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

    private struct BaseStats
    {
        public float damage;
        public int pierceCount;
        public float fireCooldown;

        public bool hasSpreadShot;
        public float spreadAngle;

        public int bounceCount;
        public float bounceSearchRadius;

        public bool hasExplodingBullets;
        public float explosionRadius;
        public float explosionDamageMultiplier;

        public bool hasFreezeBullets;
        public float freezeDuration;
        public float freezeSlowMultiplier;

        public bool hasBurnBullets;
        public float burnDuration;
        public float burnTickDamage;
        public float burnTickInterval;

        public float moveSpeed;
        public int maxLives;
        public float magnetRadius;

        public bool hasDash;
        public float dashSpeed;
        public float dashDuration;
        public float dashCooldown;

        public bool hasRadialWeapon;
        public float radialOrbitRadius;
        public float radialOrbitSpeed;
        public float radialDamageMultiplier;

        public bool hasAttackPet;
        public bool hasSupportPet;
        public float petOrbitRadius;
        public float petOrbitSpeed;
    }
}