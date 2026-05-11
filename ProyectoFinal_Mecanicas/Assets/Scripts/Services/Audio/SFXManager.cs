using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Combat")]
    public AudioClip shootClip;
    public AudioClip enemyHitClip;
    public AudioClip playerHitClip;
    public AudioClip playerDeathClip;
    public AudioClip enemyDeathClip;
    public AudioClip eliteEnemyDeathClip;



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

    [Header("Global Settings")]
    [Range(0f, 1f)] public float volume = 0.8f;

    [Header("Shoot Settings")]
    [Range(0f, 1f)] public float shootVolume = 0.65f;
    public float shootMinPitch = 0.9f;
    public float shootMaxPitch = 1.1f;

    [Header("Enemy Hit Settings")]
    [Range(0f, 1f)] public float enemyHitVolume = 0.8f;
    public float enemyHitMinPitch = 0.85f;
    public float enemyHitMaxPitch = 1.15f;

    [Header("XP Pickup Settings")]
    [Range(0f, 1f)] public float xpPickupVolume = 0.45f;
    public float xpPickupMinPitch = 0.95f;
    public float xpPickupMaxPitch = 1.15f;
    public float xpPickupSoundCooldown = 0.06f;

    private float lastXPPickupSoundTime = -999f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        PlayOneShotPitched(clip, volume, 1f, 1f);
    }

    public void Play(AudioClip clip, float customVolume)
    {
        PlayOneShotPitched(clip, customVolume, 1f, 1f);
    }

    public void PlayWithPitch(AudioClip clip, float customVolume, float minPitch, float maxPitch)
    {
        PlayOneShotPitched(clip, customVolume, minPitch, maxPitch);
    }

    private void PlayOneShotPitched(AudioClip clip, float customVolume, float minPitch, float maxPitch)
    {
        if (clip == null) return;

        float pitch = Random.Range(minPitch, maxPitch);

        GameObject sfxObject = new GameObject("SFX_" + clip.name);
        sfxObject.transform.position = transform.position;

        AudioSource source = sfxObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = customVolume;
        source.pitch = pitch;
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.loop = false;

        source.Play();

        float destroyTime = clip.length / Mathf.Abs(pitch) + 0.1f;
        Destroy(sfxObject, destroyTime);
    }

    public void PlayShoot()
    {
        PlayWithPitch(shootClip, shootVolume, shootMinPitch, shootMaxPitch);
    }

    public void PlayEnemyHit()
    {
        PlayWithPitch(enemyHitClip, enemyHitVolume, enemyHitMinPitch, enemyHitMaxPitch);
    }

    public void PlayXPPickup()
    {
        if (Time.unscaledTime - lastXPPickupSoundTime < xpPickupSoundCooldown)
            return;

        lastXPPickupSoundTime = Time.unscaledTime;

        PlayWithPitch(xpPickupClip, xpPickupVolume, xpPickupMinPitch, xpPickupMaxPitch);
    }

    public void PlayEliteEnemyDeath()
    {
        Play(eliteEnemyDeathClip);
    }
    public void PlayPlayerHit()
    {
        Play(playerHitClip);
    }

    public void PlayPlayerDeath()
    {
        Play(playerDeathClip);
    }

    public void PlayEnemyDeath()
    {
        Play(enemyDeathClip);
    }

    public void PlayLevelUp()
    {
        Play(levelUpClip);
    }

    public void PlayCardSelect()
    {
        Play(cardSelectClip);
    }

    public void PlayRecycle()
    {
        Play(recycleClip);
    }

    public void PlayButtonHover()
    {
        Play(buttonHoverClip);
    }

    public void PlayButtonClick()
    {
        Play(buttonClickClip);
    }

    public void PlayOpenDeck()
    {
        Play(openDeckClip);
    }

    public void PlayCloseDeck()
    {
        Play(closeDeckClip);
    }
}