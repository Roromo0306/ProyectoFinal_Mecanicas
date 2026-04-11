using System.Diagnostics;
using UnityEngine;

public class ActivationService : MonoBehaviour
{
    public static ActivationService Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Activate(PowerUpData data)
    {
        ApplyPowerUp(data);
    }

    private void ApplyPowerUp(PowerUpData data)
    {
        Debug.Log("Activated: " + data.title);

        // aquí se conecta con WeaponSystem / StatsSystem / AbilitySystem
    }
}