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
}