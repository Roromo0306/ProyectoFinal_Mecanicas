using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Combat")]
    public AudioClip shootClip;
    public AudioClip enemyHitClip;
    public AudioClip playerHitClip;
    public AudioClip enemyDeathClip;

    [Header("Progression")]
    public AudioClip levelUpClip;
    public AudioClip cardSelectClip;
    public AudioClip recycleClip;
    public AudioClip xpPickupClip;

    [Header("UI")]
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;
    public AudioClip openDeckClip;
    public AudioClip closeDeckClip;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 0.8f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayShoot() => Play(shootClip);
    public void PlayEnemyHit() => Play(enemyHitClip);
    public void PlayPlayerHit() => Play(playerHitClip);
    public void PlayEnemyDeath() => Play(enemyDeathClip);
    public void PlayLevelUp() => Play(levelUpClip);
    public void PlayCardSelect() => Play(cardSelectClip);
    public void PlayRecycle() => Play(recycleClip);
    public void PlayButtonHover() => Play(buttonHoverClip);
    public void PlayButtonClick() => Play(buttonClickClip);
    public void PlayOpenDeck() => Play(openDeckClip);
    public void PlayCloseDeck() => Play(closeDeckClip);

    public void PlayXPPickup() => Play(xpPickupClip);
}