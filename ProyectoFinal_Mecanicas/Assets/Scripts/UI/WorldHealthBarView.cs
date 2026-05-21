using UnityEngine;

public class WorldHealthBarView : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, -0.85f, 0f);
    [SerializeField] private bool keepHorizontal = true;

    [Header("Size")]
    [SerializeField] private float width = 1.25f;
    [SerializeField] private float height = 0.12f;

    [Header("Colors")]
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);
    [SerializeField] private Color fillColor = new Color(0.9f, 0.1f, 0.1f, 1f);

    [Header("Visibility")]
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private bool hideWhenDead = true;

    [Header("Rendering")]
    [SerializeField] private string sortingLayerName = "";
    [SerializeField] private int sortingOrder = 200;

    private const string PivotName = "WorldHealthBar_Pivot";
    private const string BackgroundName = "HealthBar_Background";
    private const string FillName = "HealthBar_Fill";

    private static Sprite cachedSprite;

    private Transform barPivot;
    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer fillRenderer;

    private IHealthStatusProvider healthProvider;
    private GameObject ownerObject;

    private void Awake()
    {
        ResolveOwner();
        BuildBar();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<DamageableHealthChangedEvent>(OnHealthChanged);
    }

    private void Start()
    {
        RefreshFromProvider();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DamageableHealthChangedEvent>(OnHealthChanged);
    }

    private void LateUpdate()
    {
        if (barPivot == null)
            return;

        barPivot.localPosition = localOffset;

        if (keepHorizontal)
            barPivot.rotation = Quaternion.identity;
    }

    private void ResolveOwner()
    {
        MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == this)
                continue;

            if (behaviour is IHealthStatusProvider provider)
            {
                healthProvider = provider;
                ownerObject = behaviour.gameObject;
                return;
            }
        }

        healthProvider = GetComponentInParent<IHealthStatusProvider>();

        if (healthProvider is MonoBehaviour providerBehaviour)
            ownerObject = providerBehaviour.gameObject;
        else
            ownerObject = gameObject;
    }

    private void BuildBar()
    {
        if (barPivot == null)
        {
            Transform existingPivot = transform.Find(PivotName);

            if (existingPivot != null)
            {
                barPivot = existingPivot;
            }
            else
            {
                GameObject pivotObject = new GameObject(PivotName);
                pivotObject.transform.SetParent(transform, false);
                barPivot = pivotObject.transform;
            }
        }

        barPivot.localPosition = localOffset;

        backgroundRenderer = GetOrCreateRenderer(BackgroundName, backgroundColor, sortingOrder);
        fillRenderer = GetOrCreateRenderer(FillName, fillColor, sortingOrder + 1);

        ApplyBarSize(1f);
    }

    private SpriteRenderer GetOrCreateRenderer(string objectName, Color color, int order)
    {
        Transform existing = barPivot.Find(objectName);
        GameObject rendererObject;

        if (existing != null)
        {
            rendererObject = existing.gameObject;
        }
        else
        {
            rendererObject = new GameObject(objectName);
            rendererObject.transform.SetParent(barPivot, false);
        }

        SpriteRenderer spriteRenderer = rendererObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = rendererObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = GetSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = order;

        if (!string.IsNullOrWhiteSpace(sortingLayerName))
            spriteRenderer.sortingLayerName = sortingLayerName;

        return spriteRenderer;
    }

    private Sprite GetSprite()
    {
        if (cachedSprite != null)
            return cachedSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.name = "WorldHealthBar_WhitePixel";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        cachedSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );

        return cachedSprite;
    }

    private void OnHealthChanged(DamageableHealthChangedEvent healthEvent)
    {
        if (healthEvent.target != ownerObject)
            return;

        SetHealth(healthEvent.currentHealth, healthEvent.maxHealth, healthEvent.isDead);
    }

    private void RefreshFromProvider()
    {
        if (healthProvider == null)
            return;

        SetHealth(
            healthProvider.CurrentHealth,
            healthProvider.MaxHealth,
            healthProvider.IsDead
        );
    }

    private void SetHealth(float currentHealth, float maxHealth, bool isDead)
    {
        float normalizedHealth = maxHealth > 0f
            ? Mathf.Clamp01(currentHealth / maxHealth)
            : 0f;

        bool shouldShow = true;

        if (hideWhenDead && isDead)
            shouldShow = false;

        if (hideWhenFull && normalizedHealth >= 0.999f)
            shouldShow = false;

        SetVisible(shouldShow);
        ApplyBarSize(normalizedHealth);
    }

    private void SetVisible(bool visible)
    {
        if (backgroundRenderer != null)
            backgroundRenderer.enabled = visible;

        if (fillRenderer != null)
            fillRenderer.enabled = visible;
    }

    private void ApplyBarSize(float normalizedHealth)
    {
        if (backgroundRenderer != null)
        {
            backgroundRenderer.transform.localPosition = Vector3.zero;
            backgroundRenderer.transform.localScale = new Vector3(width, height, 1f);
        }

        if (fillRenderer != null)
        {
            float fillWidth = width * normalizedHealth;
            float xOffset = -width * 0.5f + fillWidth * 0.5f;

            fillRenderer.transform.localPosition = new Vector3(xOffset, 0f, -0.01f);
            fillRenderer.transform.localScale = new Vector3(fillWidth, height, 1f);
        }
    }
}
