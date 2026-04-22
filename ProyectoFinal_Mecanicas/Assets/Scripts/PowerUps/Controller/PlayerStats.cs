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

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float dashCooldown = 1.5f;
    public float dashDuration = 0.2f;
    public float dashSpeedMultiplier = 2.5f;

    [Header("Survival")]
    public int maxLives = 3;

    [Header("Pickup")]
    public float magnetRadius = 2f;

    public int bounceCount = 0;
    public float bounceSearchRadius = 6f;

    public bool hasExplodingBullets = false;
    public float explosionRadius = 2.5f;
    public float explosionDamageMultiplier = 1f;
}