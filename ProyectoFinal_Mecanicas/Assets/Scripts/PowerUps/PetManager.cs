using UnityEngine;

public class PetManager : MonoBehaviour
{
    public GameObject attackPetPrefab;
    public GameObject supportPetPrefab;

    private PlayerStats playerStats;

    private GameObject currentAttackPet;
    private GameObject currentSupportPet;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Update()
    {
        if (playerStats == null)
            return;

        HandleAttackPet();
        HandleSupportPet();
    }

    private void HandleAttackPet()
    {
        if (playerStats.hasAttackPet)
        {
            if (currentAttackPet == null && attackPetPrefab != null)
            {
                currentAttackPet = Instantiate(attackPetPrefab, transform.position, Quaternion.identity);

                PetOrbiter orbiter = currentAttackPet.GetComponent<PetOrbiter>();

                if (orbiter != null)
                    orbiter.Init(transform, playerStats, 0f, true); // horario
            }
        }
        else
        {
            if (currentAttackPet != null)
            {
                Destroy(currentAttackPet);
                currentAttackPet = null;
            }
        }
    }

    private void HandleSupportPet()
    {
        if (playerStats.hasSupportPet)
        {
            if (currentSupportPet == null && supportPetPrefab != null)
            {
                currentSupportPet = Instantiate(supportPetPrefab, transform.position, Quaternion.identity);

                PetOrbiter orbiter = currentSupportPet.GetComponent<PetOrbiter>();

                if (orbiter != null)
                    orbiter.Init(transform, playerStats, 180f, false); // antihorario
            }
        }
        else
        {
            if (currentSupportPet != null)
            {
                Destroy(currentSupportPet);
                currentSupportPet = null;
            }
        }
    }
}