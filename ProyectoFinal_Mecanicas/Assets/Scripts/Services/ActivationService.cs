using UnityEngine;

public class ActivationService : MonoBehaviour
{
    public static ActivationService Instance;

    private PlayerStats playerStats;

    private void Awake()
    {
        Instance = this;
        playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null)
            Debug.LogError("ActivationService -> No se encontró PlayerStats en la escena");
    }

    public void Activate(PowerUpData data)
    {
        if (data == null)
        {
            Debug.LogError("ActivationService.Activate -> data es null");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("ActivationService.Activate -> playerStats es null");
            return;
        }

        if (data.effects == null || data.effects.Count == 0)
        {
            Debug.LogWarning("La carta " + data.title + " no tiene efectos");
            return;
        }

        foreach (var effect in data.effects)
        {
            if (effect != null)
                effect.Apply(playerStats);
        }

        Debug.Log("PowerUp activado: " + data.title);
    }
}