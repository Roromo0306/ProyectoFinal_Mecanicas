using System.Collections.Generic;
using UnityEngine;

public class CameraEnemyDensityZoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Enemy Detection")]
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float detectionPadding = 1.5f;
    [SerializeField] private int maxTrackedColliders = 250;
    [SerializeField] private float countRefreshInterval = 0.15f;

    [Header("Zoom Values")]
    [Tooltip("Si está en 0, usa el tamaño actual de la cámara al empezar.")]
    [SerializeField] private float normalOrthographicSize = 0f;

    [SerializeField] private float crowdedOrthographicSize = 6.2f;

    [Header("Enemy Amount Thresholds")]
    [SerializeField] private int lowEnemyCount = 8;
    [SerializeField] private int highEnemyCount = 35;

    [Header("Smoothing")]
    [SerializeField] private float zoomSmoothTime = 0.35f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    [SerializeField] private int visibleEnemyCount;

    private Collider2D[] detectedColliders;
    private readonly HashSet<Transform> countedEnemies = new HashSet<Transform>();

    private float countTimer;
    private float zoomVelocity;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        if (targetCamera == null)
            targetCamera = GetComponentInChildren<Camera>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError("CameraEnemyDensityZoom -> No se encontró ninguna cámara.");
            enabled = false;
            return;
        }

        if (targetCamera.orthographic && normalOrthographicSize <= 0f)
            normalOrthographicSize = targetCamera.orthographicSize;

        if (!targetCamera.orthographic && normalOrthographicSize <= 0f)
            normalOrthographicSize = targetCamera.fieldOfView;

        detectedColliders = new Collider2D[maxTrackedColliders];
    }

    private void Update()
    {
        countTimer += Time.deltaTime;

        if (countTimer >= countRefreshInterval)
        {
            countTimer = 0f;
            visibleEnemyCount = CountEnemiesInsideCamera();
        }

        UpdateCameraZoom();
    }

    private void UpdateCameraZoom()
    {
        if (targetCamera == null)
            return;

        float targetZoom = GetTargetZoom();

        if (targetCamera.orthographic)
        {
            targetCamera.orthographicSize = Mathf.SmoothDamp(
                targetCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime
            );
        }
        else
        {
            targetCamera.fieldOfView = Mathf.SmoothDamp(
                targetCamera.fieldOfView,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime
            );
        }
    }

    private float GetTargetZoom()
    {
        if (highEnemyCount <= lowEnemyCount)
            return normalOrthographicSize;

        float t = Mathf.InverseLerp(
            lowEnemyCount,
            highEnemyCount,
            visibleEnemyCount
        );

        return Mathf.Lerp(
            normalOrthographicSize,
            crowdedOrthographicSize,
            t
        );
    }

    private int CountEnemiesInsideCamera()
    {
        if (targetCamera == null)
            return 0;

        if (enemyLayers.value == 0)
            return 0;

        countedEnemies.Clear();

        Bounds cameraBounds = GetCameraWorldBounds();

        int amount = Physics2D.OverlapAreaNonAlloc(
            cameraBounds.min,
            cameraBounds.max,
            detectedColliders,
            enemyLayers
        );

        for (int i = 0; i < amount; i++)
        {
            Collider2D col = detectedColliders[i];

            if (col == null)
                continue;

            Transform enemyRoot = GetEnemyRoot(col);

            if (enemyRoot != null)
                countedEnemies.Add(enemyRoot);
        }

        if (showDebugLogs)
            Debug.Log("Enemigos visibles: " + countedEnemies.Count);

        return countedEnemies.Count;
    }

    private Bounds GetCameraWorldBounds()
    {
        Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 topRight = targetCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        Vector3 center = (bottomLeft + topRight) * 0.5f;
        Vector3 size = topRight - bottomLeft;

        size.x = Mathf.Abs(size.x) + detectionPadding * 2f;
        size.y = Mathf.Abs(size.y) + detectionPadding * 2f;
        size.z = 1f;

        return new Bounds(center, size);
    }

    private Transform GetEnemyRoot(Collider2D col)
    {
        PooledEnemyObject pooledObject = col.GetComponentInParent<PooledEnemyObject>();

        if (pooledObject != null)
            return pooledObject.transform;

        EnemyHealthSystem enemyHealth = col.GetComponentInParent<EnemyHealthSystem>();

        if (enemyHealth != null)
            return enemyHealth.transform;

        EliteEnemyHealth eliteHealth = col.GetComponentInParent<EliteEnemyHealth>();

        if (eliteHealth != null)
            return eliteHealth.transform;

        FinalBossHealth bossHealth = col.GetComponentInParent<FinalBossHealth>();

        if (bossHealth != null)
            return bossHealth.transform;

        return col.transform;
    }
}