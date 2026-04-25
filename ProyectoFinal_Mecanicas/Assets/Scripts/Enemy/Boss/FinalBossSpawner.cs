using UnityEngine;

public class FinalBossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnPoint;
    public GameTimer gameTimer;

    [SerializeField] private float spawnAfterSeconds = 600f; // 10 minutos

    private bool hasSpawned = false;

    private void Update()
    {
        if (hasSpawned)
            return;

        if (bossPrefab == null || spawnPoint == null || gameTimer == null)
        {
            Debug.LogError("FinalBossSpawner -> falta referencia");
            return;
        }

        if (gameTimer.ElapsedTime >= spawnAfterSeconds)
        {
            SpawnBoss();
        }
    }

    public void SpawnBoss()
    {
        if (hasSpawned)
            return;

        Instantiate(
            bossPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        hasSpawned = true;

        Debug.Log("BOSS FINAL SPAWNADO");
    }
}