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
        foreach (var effect in data.effects)
        {
            effect.Apply(playerStats);
        }
    }
}