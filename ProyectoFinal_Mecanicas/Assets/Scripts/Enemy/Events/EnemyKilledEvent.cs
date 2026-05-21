using UnityEngine;

public enum EnemyKillType
{
    Normal,
    Elite
}

public struct EnemyKilledEvent
{
    public GameObject enemy;
    public EnemyKillType enemyType;
    public Vector3 deathPosition;

    public EnemyKilledEvent(GameObject enemy, EnemyKillType enemyType, Vector3 deathPosition)
    {
        this.enemy = enemy;
        this.enemyType = enemyType;
        this.deathPosition = deathPosition;
    }
}
