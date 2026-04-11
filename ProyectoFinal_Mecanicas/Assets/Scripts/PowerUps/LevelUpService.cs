using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpService : MonoBehaviour
{
    public static LevelUpService Instance;

    public List<PowerUpData> allPowerUps;

    public event Action<List<PowerUpData>> OnLevelUp;

    private void Awake()
    {
        Instance = this;
    }

    public void GainXP(int xp)
    {
        // lógica XP (simplificada)
    }

    public void LevelUp()
    {
        Time.timeScale = 0f;

        List<PowerUpData> options = GetRandomOptions(3);

        OnLevelUp?.Invoke(options);
    }

    private List<PowerUpData> GetRandomOptions(int count)
    {
        List<PowerUpData> result = new List<PowerUpData>();

        while (result.Count < count)
        {
            var random = allPowerUps[UnityEngine.Random.Range(0, allPowerUps.Count)];

            if (!result.Contains(random))
                result.Add(random);
        }

        return result;
    }
}