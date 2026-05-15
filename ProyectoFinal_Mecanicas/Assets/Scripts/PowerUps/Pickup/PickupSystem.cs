using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    public float GetMagnetRadius()
    {
        if (playerStats == null) return 2f;
        return playerStats.magnetRadius;
    }
}