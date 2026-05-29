using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public GameObject Prefab { get; private set; }

    public void SetPrefab(GameObject prefab)
    {
        Prefab = prefab;
    }
}