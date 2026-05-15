using UnityEngine;

public class PetManager : MonoBehaviour
{
    [SerializeField] private GameObject attackPetPrefab;
    [SerializeField] private GameObject supportPetPrefab;

    private PlayerStats playerStats;

    private GameObject currentAttackPet;
    private GameObject currentSupportPet;

    private PetOrbiter attackPetOrbiter;
    private PetOrbiter supportPetOrbiter;

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
            EnableAttackPet();
        }
        else
        {
            DisableAttackPet();
        }
    }

    private void HandleSupportPet()
    {
        if (playerStats.hasSupportPet)
        {
            EnableSupportPet();
        }
        else
        {
            DisableSupportPet();
        }
    }

    private void EnableAttackPet()
    {
        if (currentAttackPet == null)
            CreateAttackPet();

        if (currentAttackPet == null)
            return;

        if (!currentAttackPet.activeSelf)
            currentAttackPet.SetActive(true);

        if (attackPetOrbiter != null)
            attackPetOrbiter.Init(transform, playerStats, 0f, true);
    }

    private void DisableAttackPet()
    {
        if (currentAttackPet == null)
            return;

        if (currentAttackPet.activeSelf)
            currentAttackPet.SetActive(false);
    }

    private void EnableSupportPet()
    {
        if (currentSupportPet == null)
            CreateSupportPet();

        if (currentSupportPet == null)
            return;

        if (!currentSupportPet.activeSelf)
            currentSupportPet.SetActive(true);

        if (supportPetOrbiter != null)
            supportPetOrbiter.Init(transform, playerStats, 180f, false);
    }

    private void DisableSupportPet()
    {
        if (currentSupportPet == null)
            return;

        if (currentSupportPet.activeSelf)
            currentSupportPet.SetActive(false);
    }

    private void CreateAttackPet()
    {
        if (attackPetPrefab == null)
            return;

        currentAttackPet = Instantiate(attackPetPrefab, transform.position, Quaternion.identity);
        attackPetOrbiter = currentAttackPet.GetComponent<PetOrbiter>();

        if (attackPetOrbiter != null)
            attackPetOrbiter.Init(transform, playerStats, 0f, true);
    }

    private void CreateSupportPet()
    {
        if (supportPetPrefab == null)
            return;

        currentSupportPet = Instantiate(supportPetPrefab, transform.position, Quaternion.identity);
        supportPetOrbiter = currentSupportPet.GetComponent<PetOrbiter>();

        if (supportPetOrbiter != null)
            supportPetOrbiter.Init(transform, playerStats, 180f, false);
    }
}