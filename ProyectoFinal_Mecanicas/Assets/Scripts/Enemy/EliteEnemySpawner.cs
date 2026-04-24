using UnityEngine;

public class EliteEnemySpawner : MonoBehaviour
{
    public GameTimer gameTimer;
    public Camera mainCamera;
    public GameObject eliteEnemyPrefab;
    public ArenaClosureController arenaClosure;

    [SerializeField] private float spawnAfterSeconds = 300f;
    [SerializeField] private float spawnMargin = 3f;

    private bool hasSpawned = false;
    private GameObject currentElite;

    private void Update()
    {
        if (hasSpawned) return;
        if (gameTimer == null || eliteEnemyPrefab == null || mainCamera == null) return;

        if (gameTimer.ElapsedTime >= spawnAfterSeconds)
        {
            SpawnElite();
        }
    }

    private void SpawnElite()
    {
        Vector3 spawnPos = GetSpawnPositionOutsideCamera();

        currentElite = Instantiate(eliteEnemyPrefab, spawnPos, Quaternion.identity);

        EliteEnemyHealth health = currentElite.GetComponent<EliteEnemyHealth>();
        if (health != null)
            health.spawner = this;

        hasSpawned = true;

        if (arenaClosure != null)
            arenaClosure.ActivateArena();

        Debug.Log("Elite spawnado");
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
            case 0: return new Vector3(left, Random.Range(bottom, top), 0f);
            case 1: return new Vector3(right, Random.Range(bottom, top), 0f);
            case 2: return new Vector3(Random.Range(left, right), top, 0f);
            default: return new Vector3(Random.Range(left, right), bottom, 0f);
        }
    }

    public void OnEliteDefeated()
    {
        currentElite = null;

        if (arenaClosure != null)
            arenaClosure.DeactivateArena();

        Debug.Log("Elite derrotado");
    }
}