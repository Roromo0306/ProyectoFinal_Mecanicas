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
    public SpriteRenderer spriteRenderer;

    private bool isInvulnerable;
    private bool externalInvulnerable;
    private Coroutine invRoutine;
    private float lastHitTime;
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
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
            lives = playerStats.maxLives;

        UpdateLivesUI();
    }

    private void OnHit(object evt)
    {
        if (isInvulnerable || externalInvulnerable)
            return;

        if (Time.time - lastHitTime < hitCooldown)
            return;

        lastHitTime = Time.time;

        lives--;
        EventBus.Publish(new PlayerHitEvent(transform.position));
        UpdateLivesUI();

        if (hitParticles != null)
            hitParticles.Emit(3);

        CameraShakeService.Instance?.Shake(0.15f, 0.2f);
        HitStopService.Instance?.Stop(0.05f);

        if (lives <= 0)
        {
            Debug.Log("Game Over");
            return;
        }

        StartInvulnerability();
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

            yield return new WaitForSeconds(0.1f);

            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        isInvulnerable = false;
        invRoutine = null;
    }

    public void AddLives(int amount)
    {
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
    }
}