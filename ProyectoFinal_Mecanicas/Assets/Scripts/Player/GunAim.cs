using UnityEngine;

public class GunAim : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Transform gunPivot;
    public SpriteRenderer gunSpriteRenderer;

    [Header("Settings")]
    public bool spriteFacesRightByDefault = true;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null || gunPivot == null)
            return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = gunPivot.position.z;

        Vector2 direction = mouseWorldPos - gunPivot.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gunPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        bool aimingLeft = direction.x < 0f;

        if (gunSpriteRenderer != null)
        {
            if (spriteFacesRightByDefault)
                gunSpriteRenderer.flipY = aimingLeft;
            else
                gunSpriteRenderer.flipY = !aimingLeft;
        }
    }
}