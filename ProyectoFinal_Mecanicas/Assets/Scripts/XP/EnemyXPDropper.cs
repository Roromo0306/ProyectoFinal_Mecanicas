using UnityEngine;

public class EnemyXPDropper : MonoBehaviour
{
    public GameObject xpPrefab;

    private void OnDestroy()
    {
        Instantiate(xpPrefab, transform.position, Quaternion.identity);
    }
}