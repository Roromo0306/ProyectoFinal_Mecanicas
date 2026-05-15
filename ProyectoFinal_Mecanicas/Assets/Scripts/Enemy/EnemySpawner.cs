using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private bool startActive = true;
    [SerializeField] private float activationTime = 0f;

    [Header("Enemies")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;

    [Header("Tank Settings")]
    [SerializeField] private bool spawnTanks = false;

    [Range(0f, 1f)]
    [SerializeField] private float tankSpawnChance = 0.2f;

    [Header("Spawn Rate")]
    [SerializeField] private float minSpawnRate = 0.2f;
    [SerializeField] private float maxSpawnRate = 2f;

    [Header("Spawn Amount")]
    [SerializeField] private int minEnemiesPerSpawn = 1;
    [SerializeField] private int maxEnemiesPerSpawn = 5;
    [SerializeField] private float amountRampDuration = 300f;

    [Header("Phases")]
    [SerializeField] private float rampDuration = 60f;
    [SerializeField] private float peakDuration = 180f;
    [SerializeField] private float cooldownDuration = 60f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float spawnSpreadRadius = 3f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private float timer;
    private float phaseTimer;
    private float gameTimer;

    private bool isActive;

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
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("EnemySpawner -> No se encontró Player con tag Player");

        isActive = startActive;
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;

        if (!isActive)
        {
            if (gameTimer >= activationTime)
                ActivateSpawner();
            else
                return;
        }

        if (player == null)
            return;

        phaseTimer += Time.deltaTime;
        timer += Time.deltaTime;

        UpdatePhase();

        float currentSpawnRate = GetCurrentSpawnRate();

        if (timer >= currentSpawnRate)
        {
            timer = 0f;
            SpawnGroup();
        }
    }

    private void ActivateSpawner()
    {
        isActive = true;
        timer = 0f;
        phaseTimer = 0f;
        currentPhase = SpawnPhase.Ramp;

        if (showDebugLogs)
            Debug.Log(gameObject.name + " activado en segundo: " + gameTimer);
    }

    private void UpdatePhase()
    {
        switch (currentPhase)
        {
            case SpawnPhase.Ramp:
                if (phaseTimer >= rampDuration)
                {
                    phaseTimer = 0f;
                    currentPhase = SpawnPhase.Peak;
                }
                break;

            case SpawnPhase.Peak:
                if (phaseTimer >= peakDuration)
                {
                    phaseTimer = 0f;
                    currentPhase = SpawnPhase.Cooldown;
                }
                break;

            case SpawnPhase.Cooldown:
                if (phaseTimer >= cooldownDuration)
                {
                    phaseTimer = 0f;
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
                float t = rampDuration > 0f ? phaseTimer / rampDuration : 1f;
                return Mathf.Lerp(maxSpawnRate, minSpawnRate, t);

            case SpawnPhase.Peak:
                return minSpawnRate;

            case SpawnPhase.Cooldown:
                return maxSpawnRate;

            default:
                return maxSpawnRate;
        }
    }

    private int GetCurrentEnemiesPerSpawn()
    {
        float t = amountRampDuration > 0f ? gameTimer / amountRampDuration : 1f;
        t = Mathf.Clamp01(t);

        return Mathf.RoundToInt(Mathf.Lerp(minEnemiesPerSpawn, maxEnemiesPerSpawn, t));
    }

    private void SpawnGroup()
    {
        if (enemyPrefab == null)
            return;

        if (player == null)
            return;

        int amount = GetCurrentEnemiesPerSpawn();

        Vector2 baseDir = Random.insideUnitCircle.normalized;
        Vector2 basePos = (Vector2)player.position + baseDir * spawnDistance;

        for (int i = 0; i < amount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnSpreadRadius;
            Vector2 spawnPos = basePos + offset;

            GameObject prefabToSpawn = GetEnemyPrefabToSpawn();

            if (prefabToSpawn != null)
                EnemyObjectPool.Spawn(prefabToSpawn, spawnPos, Quaternion.identity);
        }
    }

    private GameObject GetEnemyPrefabToSpawn()
    {
        if (spawnTanks && tankEnemyPrefab != null)
        {
            if (Random.value <= tankSpawnChance)
                return tankEnemyPrefab;
        }

        return enemyPrefab;
    }
}