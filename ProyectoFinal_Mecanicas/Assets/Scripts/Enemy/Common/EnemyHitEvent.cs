using UnityEngine;

public struct EnemyHitEvent
{
    public GameObject enemy;
    public float damage;

    public EnemyHitEvent(GameObject enemy, float damage)
    {
        this.enemy = enemy;
        this.damage = damage;
    }
}