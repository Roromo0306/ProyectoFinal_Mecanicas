using UnityEngine;

public class GunAim : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Transform gunPivot;
    public SpriteRenderer gunSpriteRenderer;

    [Header("Flip Settings")]
    public bool spriteFacesRightByDefault = true;

    [Header("Position Fix")]
    public Vector2 rightLocalOffset = Vector2.zero;
    public Vector2 leftLocalOffset = new Vector2(0f, 0.08f);

    private Transform gunSpriteTransform;
    private Vector3 originalGunLocalPosition;
    private Vector3 originalGunLocalScale;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (gunSpriteRenderer != null)
        {
            gunSpriteTransform = gunSpriteRenderer.transform;
            originalGunLocalPosition = gunSpriteTransform.localPosition;
            originalGunLocalScale = gunSpriteTransform.localScale;
        }
    }

    private void Update()
    {
        if (mainCamera == null || gunPivot == null)
            return;

        AimAtMouse();
    }

    private void AimAtMouse()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = gunPivot.position.z;

        Vector2 direction = mouseWorldPos - gunPivot.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gunPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        bool aimingLeft = direction.x < 0f;

        ApplyGunFlipAndOffset(aimingLeft);
    }

    private void ApplyGunFlipAndOffset(bool aimingLeft)
    {
        if (gunSpriteTransform == null || gunSpriteRenderer == null)
            return;

        bool shouldFlip = spriteFacesRightByDefault ? aimingLeft : !aimingLeft;

        // No tocamos flipY porque te está dando desplazamiento visual raro.
        gunSpriteRenderer.flipX = false;
        gunSpriteRenderer.flipY = false;

        Vector3 scale = originalGunLocalScale;
        scale.y = Mathf.Abs(originalGunLocalScale.y) * (shouldFlip ? -1f : 1f);
        gunSpriteTransform.localScale = scale;

        Vector2 offset = aimingLeft ? leftLocalOffset : rightLocalOffset;

        gunSpriteTransform.localPosition = originalGunLocalPosition + new Vector3(offset.x, offset.y, 0f);
    }
}