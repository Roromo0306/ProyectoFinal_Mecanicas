using UnityEngine;
using System.Collections.Generic;

public class ActivationService : MonoBehaviour
{
    public static ActivationService Instance;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private PlayerStats playerStats;
    private PlayerHealthSystem playerHealthSystem;

    private readonly HashSet<PowerUpData> consumedLifeGrantCards = new HashSet<PowerUpData>();

    private void Awake()
    {
        Instance = this;
        CacheReferences();
    }

    public void Activate(PowerUpData data)
    {
        RecalculateEquippedPowerUps();
    }

    public void RecalculateEquippedPowerUps()
    {
        CacheReferences();

        if (playerStats == null)
        {
            Debug.LogError("ActivationService -> No se ha encontrado PlayerStats.");
            return;
        }

        playerStats.ResetToBase();

        if (SelectionService.Instance == null)
        {
            SyncHealthWithStats(false);
            return;
        }

        ApplyEquippedCardEffects();

        if (showDebugLogs)
            LogEquippedStats();

        bool shouldGrantLife = HasNewLifeGrantCard();
        SyncHealthWithStats(shouldGrantLife);
    }

    private void CacheReferences()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (playerHealthSystem == null)
            playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
    }

    private void ApplyEquippedCardEffects()
    {
        foreach (PowerUpData card in SelectionService.Instance.equippedSlots)
        {
            if (card == null || card.effects == null)
                continue;

            foreach (PowerUpEffect effect in card.effects)
            {
                if (effect != null)
                    effect.Apply(playerStats);
            }
        }
    }

    private bool HasNewLifeGrantCard()
    {
        if (SelectionService.Instance == null)
            return false;

        bool foundNewLifeCard = false;

        foreach (PowerUpData card in SelectionService.Instance.equippedSlots)
        {
            if (card == null || card.effects == null)
                continue;

            if (consumedLifeGrantCards.Contains(card))
                continue;

            foreach (PowerUpEffect effect in card.effects)
            {
                if (effect is ExtraLifeEffect || effect is SupportPetEffect)
                {
                    consumedLifeGrantCards.Add(card);
                    foundNewLifeCard = true;
                    break;
                }
            }
        }

        return foundNewLifeCard;
    }

    private void SyncHealthWithStats(bool grantDifference)
    {
        CacheReferences();

        if (playerHealthSystem != null)
            playerHealthSystem.SyncMaxLivesFromStats(grantDifference);
    }

    private void LogEquippedStats()
    {
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
}