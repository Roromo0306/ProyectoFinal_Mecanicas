using UnityEngine;

public class BossPupilFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Auto Find Player")]
    public bool findPlayerAutomatically = true;
    public string playerTag = "Player";
    public string playerObjectName = "Player";
    public float searchInterval = 0.5f;

    [Header("Eye Root")]
    [Tooltip("Normalmente puede quedarse vacío. Si está vacío, usa el padre de la pupila como centro del ojo.")]
    public Transform eyeRoot;

    [Header("Center")]
    [Tooltip("Si está activado, usa la posición inicial de la pupila como centro.")]
    public bool useInitialLocalPositionAsCenter = true;

    [Tooltip("Solo se usa si Use Initial Local Position As Center está desactivado.")]
    public Vector2 centerLocalPosition;

    [Header("Limit")]
    [Tooltip("Límite horizontal y vertical de movimiento de la pupila.")]
    public Vector2 maxLocalOffset = new Vector2(0.2f, 0.12f);

    [Tooltip("Si está activado, la pupila siempre se mueve hasta el borde del límite.")]
    public bool alwaysLookAtLimit = true;

    [Tooltip("Solo se usa si Always Look At Limit está desactivado.")]
    public float distanceToReachLimit = 4f;

    [Header("Movement")]
    public float smoothSpeed = 12f;

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public Color gizmoColor = new Color(0f, 1f, 1f, 0.9f);

    private Vector3 initialLocalPosition;
    private bool hasInitialPosition = false;

    private float nextSearchTime = 0f;

    private void Awake()
    {
        StoreInitialPosition();
    }

    private void OnEnable()
    {
        StoreInitialPosition();
        TryFindPlayer();
    }

    private void Start()
    {
        TryFindPlayer();
    }

    private void LateUpdate()
    {
        if (player == null && findPlayerAutomatically && Time.time >= nextSearchTime)
        {
            nextSearchTime = Time.time + searchInterval;
            TryFindPlayer();
        }

        if (player == null)
            return;

        Transform root = GetEyeRoot();

        if (root == null)
            return;

        Vector3 centerLocal = GetRuntimeCenterLocalPosition();
        Vector3 playerLocal = root.InverseTransformPoint(player.position);

        Vector2 direction = new Vector2(
            playerLocal.x - centerLocal.x,
            playerLocal.y - centerLocal.y
        );

        Vector2 targetOffset = Vector2.zero;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Vector2 safeOffset = GetSafeMaxOffset();
            Vector2 directionNormalized = direction.normalized;

            Vector2 edgeOffset = GetEllipseEdgeOffset(directionNormalized, safeOffset);

            float amount = 1f;

            if (!alwaysLookAtLimit)
            {
                if (distanceToReachLimit <= 0.001f)
                    amount = 1f;
                else
                    amount = Mathf.Clamp01(direction.magnitude / distanceToReachLimit);
            }

            targetOffset = edgeOffset * amount;
        }

        Vector3 targetLocalPosition = new Vector3(
            centerLocal.x + targetOffset.x,
            centerLocal.y + targetOffset.y,
            transform.localPosition.z
        );

        if (smoothSpeed <= 0f)
        {
            transform.localPosition = targetLocalPosition;
        }
        else
        {
            float lerpAmount = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetLocalPosition,
                lerpAmount
            );
        }
    }

    private void TryFindPlayer()
    {
        if (!findPlayerAutomatically)
            return;

        if (player != null)
            return;

        GameObject playerObject = null;

        if (!string.IsNullOrEmpty(playerTag))
        {
            try
            {
                playerObject = GameObject.FindGameObjectWithTag(playerTag);
            }
            catch
            {
                playerObject = null;
            }
        }

        if (playerObject == null && !string.IsNullOrEmpty(playerObjectName))
            playerObject = GameObject.Find(playerObjectName);

        if (playerObject == null)
            playerObject = FindObjectContainingName("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private GameObject FindObjectContainingName(string namePart)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        for (int i = 0; i < allObjects.Length; i++)
        {
            GameObject currentObject = allObjects[i];

            if (currentObject == null)
                continue;

            if (!currentObject.activeInHierarchy)
                continue;

            if (currentObject.name.Contains(namePart))
                return currentObject;
        }

        return null;
    }

    private void StoreInitialPosition()
    {
        if (hasInitialPosition)
            return;

        initialLocalPosition = transform.localPosition;
        hasInitialPosition = true;
    }

    private Transform GetEyeRoot()
    {
        if (eyeRoot != null)
            return eyeRoot;

        return transform.parent;
    }

    private Vector3 GetRuntimeCenterLocalPosition()
    {
        if (useInitialLocalPositionAsCenter)
        {
            if (!hasInitialPosition)
                StoreInitialPosition();

            return initialLocalPosition;
        }

        return new Vector3(
            centerLocalPosition.x,
            centerLocalPosition.y,
            transform.localPosition.z
        );
    }

    private Vector3 GetGizmoCenterLocalPosition()
    {
        if (useInitialLocalPositionAsCenter)
            return transform.localPosition;

        return new Vector3(
            centerLocalPosition.x,
            centerLocalPosition.y,
            transform.localPosition.z
        );
    }

    private Vector2 GetSafeMaxOffset()
    {
        return new Vector2(
            Mathf.Max(0.001f, Mathf.Abs(maxLocalOffset.x)),
            Mathf.Max(0.001f, Mathf.Abs(maxLocalOffset.y))
        );
    }

    private Vector2 GetEllipseEdgeOffset(Vector2 directionNormalized, Vector2 radius)
    {
        float x = directionNormalized.x;
        float y = directionNormalized.y;

        float denominator = Mathf.Sqrt(
            (x * x) / (radius.x * radius.x) +
            (y * y) / (radius.y * radius.y)
        );

        if (denominator <= 0.0001f)
            return Vector2.zero;

        return directionNormalized / denominator;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Transform root = GetEyeRoot();

        if (root == null)
            return;

        Vector3 centerLocal = GetGizmoCenterLocalPosition();
        Vector2 safeOffset = GetSafeMaxOffset();

        Gizmos.color = gizmoColor;

        int segments = 64;

        Vector3 previousPoint = GetEllipseWorldPoint(root, centerLocal, safeOffset, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI * 2f;
            Vector3 nextPoint = GetEllipseWorldPoint(root, centerLocal, safeOffset, angle);

            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }

        Vector3 centerWorld = root.TransformPoint(centerLocal);
        Gizmos.DrawSphere(centerWorld, 0.035f);

        Gizmos.DrawLine(
            centerWorld,
            root.TransformPoint(centerLocal + new Vector3(safeOffset.x, 0f, 0f))
        );

        Gizmos.DrawLine(
            centerWorld,
            root.TransformPoint(centerLocal + new Vector3(0f, safeOffset.y, 0f))
        );
    }

    private Vector3 GetEllipseWorldPoint(Transform root, Vector3 centerLocal, Vector2 radius, float angle)
    {
        Vector3 localPoint = centerLocal + new Vector3(
            Mathf.Cos(angle) * radius.x,
            Mathf.Sin(angle) * radius.y,
            0f
        );

        return root.TransformPoint(localPoint);
    }
}