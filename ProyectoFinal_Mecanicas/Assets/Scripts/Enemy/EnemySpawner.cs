using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float minSpawnRate = 0.2f;
    public float maxSpawnRate = 2f;

    public float rampDuration = 60f;
    public float peakDuration = 180f;
    public float cooldownDuration = 60f;

    public float spawnDistance = 10f;

    private float timer;
    private float phaseTimer;

    private enum SpawnPhase
    {
        Ramp,
        Peak,
        Cooldown
    }

    private SpawnPhase currentPhase = SpawnPhase.Ramp;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        phaseTimer += Time.deltaTime;
        timer += Time.deltaTime;

        UpdatePhase();

        float currentSpawnRate = GetCurrentSpawnRate();

        if (timer >= currentSpawnRate)
        {
            timer = 0;
            Spawn();
        }
    }

    private void UpdatePhase()
    {
        switch (currentPhase)
        {
            case SpawnPhase.Ramp:
                if (phaseTimer >= rampDuration)
                {
                    phaseTimer = 0;
                    currentPhase = SpawnPhase.Peak;
                }
                break;

            case SpawnPhase.Peak:
                if (phaseTimer >= peakDuration)
                {
                    phaseTimer = 0;
                    currentPhase = SpawnPhase.Cooldown;
                }
                break;

            case SpawnPhase.Cooldown:
                if (phaseTimer >= cooldownDuration)
                {
                    phaseTimer = 0;
                    currentPhase = SpawnPhase.Ramp;
                }
                break;
        }
    }

    private float GetCurrentSpawnRate()
    {
        switch (currentPhase)
        {
            case SpawnPhase.Ramp:
                float t = phaseTimer / rampDuration;
                return Mathf.Lerp(maxSpawnRate, minSpawnRate, t);

            case SpawnPhase.Peak:
                return minSpawnRate;

            case SpawnPhase.Cooldown:
                return maxSpawnRate;

            default:
                return maxSpawnRate;
        }
    }

    private void Spawn()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector2 pos = (Vector2)player.position + dir * spawnDistance;

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}