using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerHealthSystem : MonoBehaviour
{
    public int lives = 3;

    public float invulnerabilityTime = 1.5f;
    public float hitCooldown = 0.2f;

    public TextMeshProUGUI livesText;
    public ParticleSystem hitParticles;
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

    private void OnHit(object evt)
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

        yield return new WaitForSecondsRealtime(0.25f);

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
        else
            Debug.LogWarning("PlayerHealthSystem -> livesText no asignado");
    }
}