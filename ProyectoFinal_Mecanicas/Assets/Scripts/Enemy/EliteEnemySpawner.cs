using UnityEngine;

public class EliteEnemySpawner : MonoBehaviour
{
    public GameTimer gameTimer;
    public Camera mainCamera;
    public GameObject eliteEnemyPrefab;
    public ArenaClosureController arenaClosure;

    [Header("Spawn Timing")]
    [SerializeField] private float firstSpawnAfterSeconds = 150f;
    [SerializeField] private float spawnInterval = 90f;

    [Header("Spawn Limits")]
    [SerializeField] private int maxElitesAlive = 1;
    [SerializeField] private int maxElitesTotal = 5;

    [Header("Spawn Position")]
    [SerializeField] private float spawnMargin = 3f;

    private float nextSpawnTime;
    private int aliveElites = 0;
    private int spawnedElitesTotal = 0;

    private void Start()
    {
        nextSpawnTime = firstSpawnAfterSeconds;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (gameTimer == null || eliteEnemyPrefab == null || mainCamera == null)
            return;

        if (spawnedElitesTotal >= maxElitesTotal)
            return;

        if (aliveElites >= maxElitesAlive)
            return;

        if (gameTimer.ElapsedTime >= nextSpawnTime)
        {
            SpawnElite();
            nextSpawnTime += spawnInterval;
        }
    }

    private void SpawnElite()
    {
        Vector3 spawnPos = GetSpawnPositionOutsideCamera();

        GameObject elite = Instantiate(eliteEnemyPrefab, spawnPos, Quaternion.identity);

        EliteEnemyHealth health = elite.GetComponent<EliteEnemyHealth>();
        if (health != null)
            health.spawner = this;

        aliveElites++;
        spawnedElitesTotal++;

        if (arenaClosure != null)
            arenaClosure.ActivateArena();

        Debug.Log("Elite spawnado. Vivos: " + aliveElites + " | Total: " + spawnedElitesTotal);
    }

    private Vector3 GetSpawnPositionOutsideCamera()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float left = bottomLeft.x - spawnMargin;
        float right = topRight.x + spawnMargin;
        float bottom = bottomLeft.y - spawnMargin;
        float top = topRight.y + spawnMargin;

        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                return new Vector3(left, Random.Range(bottom, top), 0f);

            case 1:
                return new Vector3(right, Random.Range(bottom, top), 0f);

            case 2:
                return new Vector3(Random.Range(left, right), top, 0f);

            default:
                return new Vector3(Random.Range(left, right), bottom, 0f);
        }
    }

    public void OnEliteDefeated()
    {
        aliveElites = Mathf.Max(0, aliveElites - 1);

        if (arenaClosure != null && aliveElites <= 0)
            arenaClosure.DeactivateArena();

        Debug.Log("Elite derrotado. Vivos restantes: " + aliveElites);
    }
}