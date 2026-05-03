using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Activation")]
    public bool startActive = true;
    public float activationTime = 0f;

    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Rate")]
    public float minSpawnRate = 0.2f;
    public float maxSpawnRate = 2f;

    [Header("Phases")]
    public float rampDuration = 60f;
    public float peakDuration = 180f;
    public float cooldownDuration = 60f;

    [Header("Spawn Position")]
    public float spawnDistance = 10f;

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

        if (!startActive)
            gameObject.SetActive(true);
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;

        if (!isActive)
        {
            if (gameTimer >= activationTime)
            {
                ActivateSpawner();
            }
            else
            {
                return;
            }
        }

        if (player == null) return;

        phaseTimer += Time.deltaTime;
        timer += Time.deltaTime;

        UpdatePhase();

        float currentSpawnRate = GetCurrentSpawnRate();

        if (timer >= currentSpawnRate)
        {
            timer = 0f;
            Spawn();
        }
    }

    private void ActivateSpawner()
    {
        isActive = true;
        timer = 0f;
        phaseTimer = 0f;
        currentPhase = SpawnPhase.Ramp;

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

    private void Spawn()
    {
        if (enemyPrefab == null) return;
        if (player == null) return;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector2 pos = (Vector2)player.position + dir * spawnDistance;

        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}