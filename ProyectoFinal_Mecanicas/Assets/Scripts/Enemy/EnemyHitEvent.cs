using UnityEngine;

public struct EnemyHitEvent
{
    public GameObject enemy;

    public EnemyHitEvent(GameObject e)
    {
        enemy = e;
    }
}
