using UnityEngine;

public class ActivationService : MonoBehaviour
{
    public static ActivationService Instance;

    private PlayerStats playerStats;

    private void Awake()
    {
        Instance = this;
        playerStats = FindObjectOfType<PlayerStats>();
    }

    public void Activate(PowerUpData data)
    {
        RecalculateEquippedPowerUps();
    }

    public void RecalculateEquippedPowerUps()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null) return;

        ResetStatsToBase();

        if (SelectionService.Instance == null) return;

        foreach (PowerUpData card in SelectionService.Instance.equippedSlots)
        {
            if (card == null || card.effects == null) continue;

            foreach (PowerUpEffect effect in card.effects)
            {
                if (effect != null)
                    effect.Apply(playerStats);
            }
        }

        Debug.Log(
            "EQUIPADAS -> " +
            "Damage " + playerStats.damage +
            " | Pierce " + playerStats.pierceCount +
            " | Bounce " + playerStats.bounceCount +
            " | Spread " + playerStats.hasSpreadShot +
            " | Explosion " + playerStats.hasExplodingBullets +
            " | Burn " + playerStats.hasBurnBullets +
            " | Freeze " + playerStats.hasFreezeBullets +
            " | AttackPet " + playerStats.hasAttackPet +
            " | SupportPet " + playerStats.hasSupportPet
        );
    }

    private void ResetStatsToBase()
    {
        playerStats.damage = 1f;
        playerStats.pierceCount = 1;
        playerStats.fireCooldown = 0.3f;

        playerStats.hasSpreadShot = false;
        playerStats.spreadAngle = 20f;

        playerStats.bounceCount = 0;
        playerStats.bounceSearchRadius = 6f;

        playerStats.hasExplodingBullets = false;
        playerStats.explosionRadius = 2.5f;
        playerStats.explosionDamageMultiplier = 1f;

        playerStats.hasFreezeBullets = false;
        playerStats.freezeDuration = 2f;
        playerStats.freezeSlowMultiplier = 0.4f;

        playerStats.hasBurnBullets = false;
        playerStats.burnDuration = 3f;
        playerStats.burnTickDamage = 0.2f;
        playerStats.burnTickInterval = 0.4f;

        playerStats.moveSpeed = 5f;
        playerStats.maxLives = 3;
        playerStats.magnetRadius = 2f;

        playerStats.hasDash = false;
        playerStats.dashSpeed = 18f;
        playerStats.dashDuration = 0.18f;
        playerStats.dashCooldown = 1.2f;

        playerStats.hasRadialWeapon = false;
        playerStats.radialOrbitRadius = 1.8f;
        playerStats.radialOrbitSpeed = 180f;
        playerStats.radialDamageMultiplier = 1f;

        playerStats.hasAttackPet = false;
        playerStats.hasSupportPet = false;
        playerStats.petOrbitRadius = 2.2f;
        playerStats.petOrbitSpeed = 160f;
    }
}