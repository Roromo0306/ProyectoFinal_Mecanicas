using UnityEngine;
using UnityEngine.UI;

public class GameCursor : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public RectTransform cursorRect;
    public Image cursorImage;

    [Header("Sprites")]
    public Sprite normalCursorSprite;
    public Sprite targetCursorSprite;

    [Header("Blink")]
    public float blinkSpeed = 10f;
    public Color normalColor = Color.white;
    public Color targetColor = Color.red;

    [Header("Detection")]
    public LayerMask enemyLayer;

    private Canvas canvas;
    private bool isOverEnemy;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        canvas = GetComponentInParent<Canvas>();

        Cursor.visible = false;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        FollowMouse();
        CheckEnemyUnderCursor();
        UpdateCursorVisual();
    }

    private void FollowMouse()
    {
        if (cursorRect == null || canvas == null)
            return;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out localPoint
        );

        cursorRect.anchoredPosition = localPoint;
    }

    private void CheckEnemyUnderCursor()
    {
        isOverEnemy = false;

        if (mainCamera == null)
            return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            if (hit.GetComponentInParent<EnemyHealthSystem>() != null)
            {
                isOverEnemy = true;
                return;
            }

            if (hit.GetComponentInParent<EliteEnemyHealth>() != null)
            {
                isOverEnemy = true;
                return;
            }

            if (hit.GetComponentInParent<FinalBossHealth>() != null)
            {
                isOverEnemy = true;
                return;
            }
        }
    }

    private void UpdateCursorVisual()
    {
        if (cursorImage == null)
            return;

        if (isOverEnemy)
        {
            if (targetCursorSprite != null)
                cursorImage.sprite = targetCursorSprite;

            float blink = Mathf.Abs(Mathf.Sin(Time.unscaledTime * blinkSpeed));
            cursorImage.color = Color.Lerp(normalColor, targetColor, blink);
        }
        else
        {
            if (normalCursorSprite != null)
                cursorImage.sprite = normalCursorSprite;

            cursorImage.color = normalColor;
        }
    }
}