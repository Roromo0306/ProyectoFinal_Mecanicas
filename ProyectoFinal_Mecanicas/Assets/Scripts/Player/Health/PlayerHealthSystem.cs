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

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

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
        if (playerStats != null)
        {
            lives = playerStats.maxLives;
            lastKnownMaxLives = playerStats.maxLives;
        }

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
            return;

        int newMaxLives = playerStats.maxLives;
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

        lives += amount;

        if (playerStats != null)
            lives = Mathf.Min(lives, playerStats.maxLives);

        UpdateLivesUI();

        Debug.Log("Vidas actuales: " + lives + " / " + (playerStats != null ? playerStats.maxLives : lives));
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

        int maxLives = lives;

        if (playerStats != null)
            maxLives = playerStats.maxLives;

        while (heartsContainer.childCount < maxLives)
        {
            Instantiate(heartPrefab, heartsContainer);
        }

        while (heartsContainer.childCount > maxLives)
        {
            Transform lastHeart = heartsContainer.GetChild(heartsContainer.childCount - 1);
            Destroy(lastHeart.gameObject);
        }

        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            Image heartImage = heartsContainer.GetChild(i).GetComponent<Image>();

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

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.maxLives += amount;
            lastKnownMaxLives = playerStats.maxLives;
        }

        lives += amount;

        if (playerStats != null)
            lives = Mathf.Min(lives, playerStats.maxLives);

        UpdateLivesUI();

        Debug.Log("Coraz�n ganado -> " + lives + " / " + (playerStats != null ? playerStats.maxLives : lives));
    }
}