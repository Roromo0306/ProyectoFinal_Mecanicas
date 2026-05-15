using UnityEngine;

public class RadialWeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject radialWeaponPrefab;

    private PlayerStats playerStats;
    private GameObject currentOrbital;
    private RadialOrbiter currentOrbiter;

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
            EnableOrbital();
        }
        else
        {
            DisableOrbital();
        }
    }

    private void EnableOrbital()
    {
        if (currentOrbital == null)
            CreateOrbital();

        if (currentOrbital == null)
            return;

        if (!currentOrbital.activeSelf)
            currentOrbital.SetActive(true);

        if (currentOrbiter != null)
            currentOrbiter.Init(transform, playerStats, 0f);
    }

    private void DisableOrbital()
    {
        if (currentOrbital == null)
            return;

        if (currentOrbital.activeSelf)
            currentOrbital.SetActive(false);
    }

    private void CreateOrbital()
    {
        currentOrbital = Instantiate(radialWeaponPrefab, transform.position, Quaternion.identity);
        currentOrbiter = currentOrbital.GetComponent<RadialOrbiter>();

        if (currentOrbiter != null)
            currentOrbiter.Init(transform, playerStats, 0f);
    }
}