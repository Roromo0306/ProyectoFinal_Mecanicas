using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            timer = 0;
            Spawn();
        }
    }

    private void Spawn()
    {
        Vector2 pos = Random.insideUnitCircle * 8f;
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}