using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{
    public int lives = 3;

    public float invulnerabilityTime = 1.5f;
    public float hitCooldown = 0.2f;

    [Header("Old Text UI")]
    public TextMeshProUGUI livesText;

    [Header("Heart UI")]
    public Transform heartsContainer;
    public GameObject heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("Hit Feedback")]
    public ParticleSystem hitParticles;

    [Header("Death Feedback")]
    public ParticleSystem deathParticles;
    public GameObject deathEffectPrefab;
    public float deathDelayBeforePanel = 0.45f;
    public bool hideSpriteOnDeath = true;

    private SpriteRenderer spriteRenderer;

    private bool isInvulnerable;
    private bool externalInvulnerable;
    private bool isDead;

    private Coroutine invRoutine;
    private Coroutine flashRoutine;

    private float lastHitTime = -999f;
    private PlayerStats playerStats;
    private int lastKnownMaxLives;

    // Corazones ganados por élites.
    // IMPORTANTE: esto va separado de PlayerStats porque ActivationService hace ResetToBase().
    private int eliteBonusMaxLives = 0;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHitEvent>(OnHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHitEvent>(OnHit);
    }

    private void Start()
    {
        int maxLives = GetEffectiveMaxLives();

        lives = Mathf.Clamp(lives, 0, maxLives);

        if (lives <= 0)
            lives = maxLives;

        lastKnownMaxLives = maxLives;

        UpdateLivesUI();
    }

    private void OnHit(PlayerHitEvent evt)
    {
        TakeDamage();
    }

    public void ForceDamage()
    {
        TakeDamage();
    }

    public void SyncMaxLivesFromStats(bool grantDifference)
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        int newMaxLives = GetEffectiveMaxLives();
        int difference = newMaxLives - lastKnownMaxLives;

        if (grantDifference && difference > 0)
            lives += difference;

        lives = Mathf.Clamp(lives, 0, newMaxLives);
        lastKnownMaxLives = newMaxLives;

        UpdateLivesUI();

        Debug.Log("SYNC VIDAS -> " + lives + " / " + newMaxLives + " | grant: " + grantDifference);
    }

    private void TakeDamage()
    {
        if (isDead)
            return;

        if (isInvulnerable || externalInvulnerable)
            return;

        if (Time.time - lastHitTime < hitCooldown)
            return;

        lastHitTime = Time.time;

        lives--;
        lives = Mathf.Max(0, lives);

        SFXManager.Instance?.PlayPlayerHit();

        UpdateLivesUI();
        PlayHitFeedback();

        if (lives <= 0)
        {
            StartCoroutine(DeathRoutine());
            return;
        }

        StartInvulnerability();
    }

    private void PlayHitFeedback()
    {
        if (hitParticles != null)
            hitParticles.Emit(3);
        else
            Debug.LogWarning("PlayerHealthSystem -> hitParticles no asignado");

        CameraShakeService.Instance?.Shake(0.15f, 0.2f);
        HitStopService.Instance?.Stop(0.05f);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("PlayerHealthSystem -> spriteRenderer no asignado");
            yield break;
        }

        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.08f);

        spriteRenderer.color = Color.white;
        yield return new WaitForSecondsRealtime(0.08f);

        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.08f);

        spriteRenderer.color = Color.white;
    }

    private IEnumerator DeathRoutine()
    {
        if (isDead)
            yield break;

        isDead = true;

        Debug.Log("Game Over");

        if (invRoutine != null)
            StopCoroutine(invRoutine);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        SFXManager.Instance?.PlayPlayerDeath();

        CameraShakeService.Instance?.Shake(0.35f, 0.35f);
        HitStopService.Instance?.Stop(0.08f);

        if (deathParticles != null)
        {
            deathParticles.transform.position = transform.position;
            deathParticles.Play();
        }

        if (deathEffectPrefab != null)
        {
            GameObject fx = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        if (hideSpriteOnDeath && spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSecondsRealtime(deathDelayBeforePanel);

        if (EndGameUI.Instance != null)
            EndGameUI.Instance.ShowLose();
        else
            Debug.LogError("EndGameUI.Instance es null");
    }

    private void StartInvulnerability()
    {
        if (invRoutine != null)
            StopCoroutine(invRoutine);

        invRoutine = StartCoroutine(InvulnerabilityRoutine());
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        float elapsed = 0f;

        while (elapsed < invulnerabilityTime)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = Color.red;

            yield return new WaitForSecondsRealtime(0.1f);

            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;

            yield return new WaitForSecondsRealtime(0.1f);

            elapsed += 0.2f;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        isInvulnerable = false;
        invRoutine = null;
    }

    public void AddLives(int amount)
    {
        if (isDead)
            return;

        int maxLives = GetEffectiveMaxLives();

        lives += amount;
        lives = Mathf.Clamp(lives, 0, maxLives);

        UpdateLivesUI();

        Debug.Log("Vidas actuales: " + lives + " / " + maxLives);
    }

    public void SetExternalInvulnerable(bool value)
    {
        externalInvulnerable = value;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = lives.ToString();

        UpdateHeartUI();
    }

    private void UpdateHeartUI()
    {
        if (heartsContainer == null || heartPrefab == null)
            return;

        int maxLives = GetEffectiveMaxLives();

        lives = Mathf.Clamp(lives, 0, maxLives);

        int currentChildren = heartsContainer.childCount;

        // Añadir corazones si faltan.
        if (currentChildren < maxLives)
        {
            int amountToCreate = maxLives - currentChildren;

            for (int i = 0; i < amountToCreate; i++)
            {
                Instantiate(heartPrefab, heartsContainer);
            }
        }

        // Quitar corazones si sobran.
        // IMPORTANTE:
        // No usamos while, porque Destroy() no reduce childCount hasta final del frame.
        // Usar while aquí puede congelar Unity para siempre.
        currentChildren = heartsContainer.childCount;

        if (currentChildren > maxLives)
        {
            for (int i = currentChildren - 1; i >= maxLives; i--)
            {
                Transform extraHeart = heartsContainer.GetChild(i);

                if (extraHeart != null)
                {
                    extraHeart.gameObject.SetActive(false);
                    Destroy(extraHeart.gameObject);
                }
            }
        }

        // Actualizar sprites solo de los corazones válidos.
        int visibleHeartCount = Mathf.Min(heartsContainer.childCount, maxLives);

        for (int i = 0; i < visibleHeartCount; i++)
        {
            Transform heartTransform = heartsContainer.GetChild(i);

            if (heartTransform == null)
                continue;

            Image heartImage = heartTransform.GetComponent<Image>();

            if (heartImage == null)
                continue;

            if (i < lives)
                heartImage.sprite = fullHeartSprite;
            else
                heartImage.sprite = emptyHeartSprite;
        }
    }

    public void AddMaxHeartAndHeal(int amount)
    {
        if (isDead)
            return;

        if (amount <= 0)
            return;

        // No tocamos directamente playerStats.maxLives.
        // playerStats se recalcula cada vez que cambias cartas.
        // Si metemos aquí el corazón del élite, ActivationService lo puede borrar con ResetToBase().
        eliteBonusMaxLives += amount;

        int maxLives = GetEffectiveMaxLives();

        lives += amount;
        lives = Mathf.Clamp(lives, 0, maxLives);

        lastKnownMaxLives = maxLives;

        UpdateLivesUI();

        Debug.Log("Corazón ganado -> " + lives + " / " + maxLives);
    }

    private int GetEffectiveMaxLives()
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        int baseMaxLives = lives;

        if (playerStats != null)
            baseMaxLives = playerStats.maxLives;

        baseMaxLives = Mathf.Max(1, baseMaxLives);

        return baseMaxLives + eliteBonusMaxLives;
    }
}