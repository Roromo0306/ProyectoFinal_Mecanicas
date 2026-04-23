using UnityEngine;

public class RadialWeaponManager : MonoBehaviour
{
    public GameObject radialWeaponPrefab;

    private PlayerStats playerStats;
    private GameObject currentOrbital;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        if (playerStats == null || radialWeaponPrefab == null)
            return;

        if (playerStats.hasRadialWeapon)
        {
            if (currentOrbital == null)
            {
                SpawnOrbital();
            }
        }
        else
        {
            if (currentOrbital != null)
            {
                Destroy(currentOrbital);
                currentOrbital = null;
            }
        }
    }

    private void SpawnOrbital()
    {
        currentOrbital = Instantiate(radialWeaponPrefab, transform.position, Quaternion.identity);

        RadialOrbiter orbiter = currentOrbital.GetComponent<RadialOrbiter>();
        if (orbiter != null)
        {
            orbiter.Init(transform, playerStats, 0f);
        }
    }
}