using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    public float smooth = 10f;

    [Header("Camera Limits")]
    public bool useLimits = true;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPos = target.position + offset;

        if (useLimits && cam != null)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            // Diferencia entre el Empty y la cámara hija
            Vector3 cameraOffsetFromParent = cam.transform.position - transform.position;

            // Dónde quedaría la cámara realmente si movemos el Empty
            Vector3 desiredCameraPos = targetPos + cameraOffsetFromParent;

            bool validXLimits = maxX > minX && (maxX - minX) > camWidth * 2f;
            bool validYLimits = maxY > minY && (maxY - minY) > camHeight * 2f;

            if (validXLimits)
            {
                float clampedCameraX = Mathf.Clamp(
                    desiredCameraPos.x,
                    minX + camWidth,
                    maxX - camWidth
                );

                targetPos.x += clampedCameraX - desiredCameraPos.x;
            }

            if (validYLimits)
            {
                float clampedCameraY = Mathf.Clamp(
                    desiredCameraPos.y,
                    minY + camHeight,
                    maxY - camHeight
                );

                targetPos.y += clampedCameraY - desiredCameraPos.y;
            }
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smooth * Time.deltaTime
        );
    }
}