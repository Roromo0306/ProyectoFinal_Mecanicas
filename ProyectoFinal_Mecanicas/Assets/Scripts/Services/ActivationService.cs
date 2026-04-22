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
        if (data == null)
        {
            Debug.LogError("ActivationService -> data es null");
            return;
        }

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("ActivationService -> playerStats es null");
            return;
        }

        if (data.effects == null)
            return;

        foreach (var effect in data.effects)
        {
            if (effect != null)
                effect.Apply(playerStats);
        }
    }
}