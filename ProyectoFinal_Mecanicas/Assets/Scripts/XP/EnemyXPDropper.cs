using UnityEngine;

public class EnemyXPDropper : MonoBehaviour
{
    public GameObject xpPrefab;

    public void DropXP()
    {
        if (xpPrefab == null) return;
        Instantiate(xpPrefab, transform.position, Quaternion.identity);
    }
}