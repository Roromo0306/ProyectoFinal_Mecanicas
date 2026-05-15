using UnityEngine;

public class EnemyXPDropper : MonoBehaviour
{
    [Header("XP Prefabs")]
    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private GameObject bigXPPrefab;

    [Header("Late Game XP")]
    [SerializeField] private float bigXPStartTime = 300f;

    [Range(0f, 1f)]
    [SerializeField] private float bigXPDropChance = 0.65f;

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
            return;
        }

        TryDropBigXP();
    }

    private void DropNormalXP()
    {
        if (xpPrefab == null)
            return;

        XPObjectPool.Spawn(xpPrefab, transform.position, Quaternion.identity);
    }

    private void TryDropBigXP()
    {
        if (bigXPPrefab == null)
            return;

        if (Random.value <= bigXPDropChance)
            XPObjectPool.Spawn(bigXPPrefab, transform.position, Quaternion.identity);
    }

    public static void ResetRunTimer()
    {
        runStartTime = Time.time;
        runStartTimeSet = true;
    }
}