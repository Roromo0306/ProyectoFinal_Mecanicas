using UnityEngine;
using UnityEngine.UI;
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
        if (isInvulnerable)
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
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        spriteRenderer.color = Color.white;
        isInvulnerable = false;
        invRoutine = null;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = lives.ToString();
    }
}