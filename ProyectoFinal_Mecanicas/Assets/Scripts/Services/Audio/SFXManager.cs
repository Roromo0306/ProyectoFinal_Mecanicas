using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Combat")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip enemyHitClip;
    [SerializeField] private AudioClip playerHitClip;
    [SerializeField] private AudioClip playerDeathClip;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip eliteEnemyDeathClip;

    [Header("Progression")]
    [SerializeField] private AudioClip levelUpClip;
    [SerializeField] private AudioClip cardSelectClip;
    [SerializeField] private AudioClip recycleClip;
    [SerializeField] private AudioClip xpPickupClip;

    [Header("UI")]
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip openDeckClip;
    [SerializeField] private AudioClip closeDeckClip;

    [Header("Global Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;

    [SerializeField] private string sfxVolumePrefsKey = "MasterAudioVolume";

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 12;
    [SerializeField] private int maxPoolSize = 32;

    [Header("Shoot Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float shootVolume = 0.65f;
    [SerializeField] private float shootMinPitch = 0.9f;
    [SerializeField] private float shootMaxPitch = 1.1f;

    [Header("Enemy Hit Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float enemyHitVolume = 0.8f;
    [SerializeField] private float enemyHitMinPitch = 0.85f;
    [SerializeField] private float enemyHitMaxPitch = 1.15f;

    [Header("XP Pickup Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float xpPickupVolume = 0.45f;
    [SerializeField] private float xpPickupMinPitch = 0.95f;
    [SerializeField] private float xpPickupMaxPitch = 1.15f;
    [SerializeField] private float xpPickupSoundCooldown = 0.06f;

    private readonly List<AudioSource> audioSourcePool = new List<AudioSource>();

    private int forcedSourceIndex = 0;
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

        masterVolume = PlayerPrefs.GetFloat(sfxVolumePrefsKey, masterVolume);

        CreateInitialPool();
    }

    private void CreateInitialPool()
    {
        int safePoolSize = Mathf.Max(1, initialPoolSize);

        for (int i = 0; i < safePoolSize; i++)
        {
            CreateAudioSource();
        }
    }

    private AudioSource CreateAudioSource()
    {
        GameObject sourceObject = new GameObject("Pooled_SFX_Source");
        sourceObject.transform.SetParent(transform);
        sourceObject.transform.localPosition = Vector3.zero;

        AudioSource source = sourceObject.AddComponent<AudioSource>();
        ConfigureAudioSource(source);

        audioSourcePool.Add(source);

        return source;
    }

    private void ConfigureAudioSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = GetRealVolume(volume);
        source.pitch = 1f;
    }

    private AudioSource GetAvailableSource()
    {
        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            AudioSource source = audioSourcePool[i];

            if (source != null && !source.isPlaying)
                return source;
        }

        if (audioSourcePool.Count < maxPoolSize)
            return CreateAudioSource();

        return GetForcedReusableSource();
    }

    private AudioSource GetForcedReusableSource()
    {
        if (audioSourcePool.Count == 0)
            return CreateAudioSource();

        forcedSourceIndex++;

        if (forcedSourceIndex >= audioSourcePool.Count)
            forcedSourceIndex = 0;

        AudioSource source = audioSourcePool[forcedSourceIndex];

        if (source != null)
        {
            source.Stop();
            return source;
        }

        source = CreateAudioSource();
        return source;
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
        if (clip == null)
            return;

        AudioSource source = GetAvailableSource();

        if (source == null)
            return;

        float safeMinPitch = Mathf.Min(minPitch, maxPitch);
        float safeMaxPitch = Mathf.Max(minPitch, maxPitch);

        source.Stop();

        source.clip = clip;
        source.volume = GetRealVolume(customVolume);
        source.pitch = Random.Range(safeMinPitch, safeMaxPitch);
        source.spatialBlend = 0f;
        source.loop = false;
        source.Play();
    }

    private float GetRealVolume(float baseVolume)
    {
        return Mathf.Clamp01(baseVolume * masterVolume);
    }

    public void SetMasterVolume(float value)
    {
        float previousMasterVolume = masterVolume;

        masterVolume = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(sfxVolumePrefsKey, masterVolume);
        PlayerPrefs.Save();

        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            AudioSource source = audioSourcePool[i];

            if (source == null || !source.isPlaying)
                continue;

            if (previousMasterVolume > 0.001f)
                source.volume = Mathf.Clamp01((source.volume / previousMasterVolume) * masterVolume);
            else
                source.volume = GetRealVolume(volume);
        }
    }

    public float GetMasterVolume()
    {
        return masterVolume;
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