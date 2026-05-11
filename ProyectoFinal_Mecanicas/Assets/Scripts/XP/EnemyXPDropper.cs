using UnityEngine;

public class EnemyXPDropper : MonoBehaviour
{
    [Header("XP Prefabs")]
    public GameObject xpPrefab;
    public GameObject bigXPPrefab;

    [Header("Late Game XP")]
    public float bigXPStartTime = 300f; // minuto 5
    [Range(0f, 1f)] public float bigXPDropChance = 0.65f;

    private static float runStartTime;
    private static bool runStartTimeSet = false;

    private void Awake()
    {
        if (!runStartTimeSet)
        {
            runStartTime = Time.time;
            runStartTimeSet = true;
        }
    }

    public void DropXP()
    {
        float elapsedTime = Time.time - runStartTime;

        if (elapsedTime < bigXPStartTime)
        {
            DropNormalXP();
        }
        else
        {
            TryDropBigXP();
        }
    }

    private void DropNormalXP()
    {
        if (xpPrefab == null) return;

        Instantiate(xpPrefab, transform.position, Quaternion.identity);
    }

    private void TryDropBigXP()
    {
        if (bigXPPrefab == null) return;

        if (Random.value <= bigXPDropChance)
        {
            Instantiate(bigXPPrefab, transform.position, Quaternion.identity);
        }
    }

    public static void ResetRunTimer()
    {
        runStartTime = Time.time;
        runStartTimeSet = true;
    }
}