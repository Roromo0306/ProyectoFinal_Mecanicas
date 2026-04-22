using UnityEngine;
using UnityEngine.EventSystems;

public class CardBalatroVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform visualRoot;
    [SerializeField] private DragCard dragCard;

    [Header("Hover")]
    [SerializeField] private float hoverScale = 1.04f;
    [SerializeField] private float maxTiltX = 6f;
    [SerializeField] private float maxTiltY = 6f;
    [SerializeField] private float smooth = 12f;

    [Header("Idle")]
    [SerializeField] private float idleRock = 2f;
    [SerializeField] private float idleRockSpeed = 1.3f;

    [Header("Drag")]
    [SerializeField] private float dragScale = 1.08f;
    [SerializeField] private float dragTiltStrength = 0.15f;
    [SerializeField] private float maxDragTiltX = 10f;
    [SerializeField] private float maxDragTiltY = 10f;
    [SerializeField] private float maxDragZ = 8f;

    private bool isHovering;
    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 targetScale = Vector3.one;

    private Vector3 lastMousePosition;
    private Vector3 mouseDelta;

    private void Awake()
    {
        if (visualRoot == null)
            Debug.LogError("CardBalatroVisual -> falta visualRoot en " + gameObject.name);

        if (dragCard == null)
            dragCard = GetComponent<DragCard>();

        lastMousePosition = Input.mousePosition;
    }

    private void LateUpdate()
    {
        if (visualRoot == null) return;

        mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        bool isDragging = dragCard != null && dragCard.IsDragging();

        if (isDragging)
        {
            UpdateDragVisual();
        }
        else
        {
            UpdateRotation();
            UpdateScale();
        }

        visualRoot.localRotation = Quaternion.Slerp(
            visualRoot.localRotation,
            targetRotation,
            Time.unscaledDeltaTime * smooth
        );

        visualRoot.localScale = Vector3.Lerp(
            visualRoot.localScale,
            targetScale,
            Time.unscaledDeltaTime * smooth
        );
    }

    private void UpdateRotation()
    {
        float idleZ = Mathf.Sin(Time.unscaledTime * idleRockSpeed + transform.GetSiblingIndex()) * idleRock;

        if (!isHovering)
        {
            targetRotation = Quaternion.Euler(0f, 0f, idleZ);
            return;
        }

        Vector2 localMouse;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            visualRoot,
            Input.mousePosition,
            null,
            out localMouse
        );

        float halfW = Mathf.Max(1f, visualRoot.rect.width * 0.5f);
        float halfH = Mathf.Max(1f, visualRoot.rect.height * 0.5f);

        float x = Mathf.Clamp(localMouse.x / halfW, -1f, 1f);
        float y = Mathf.Clamp(localMouse.y / halfH, -1f, 1f);

        float rotX = -y * maxTiltX;
        float rotY = x * maxTiltY;

        targetRotation = Quaternion.Euler(rotX, rotY, idleZ);
    }

    private void UpdateScale()
    {
        targetScale = isHovering ? Vector3.one * hoverScale : Vector3.one;
    }

    private void UpdateDragVisual()
    {
        float tiltX = Mathf.Clamp(-mouseDelta.y * dragTiltStrength, -maxDragTiltX, maxDragTiltX);
        float tiltY = Mathf.Clamp(mouseDelta.x * dragTiltStrength, -maxDragTiltY, maxDragTiltY);
        float rotZ = Mathf.Clamp(mouseDelta.x * 0.08f, -maxDragZ, maxDragZ);

        targetRotation = Quaternion.Euler(tiltX, tiltY, rotZ);
        targetScale = Vector3.one * dragScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void ResetVisual()
    {
        isHovering = false;

        if (visualRoot == null) return;

        visualRoot.localRotation = Quaternion.identity;
        visualRoot.localScale = Vector3.one;
    }
}