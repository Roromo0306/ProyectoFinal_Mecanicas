using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip gameOverMusic;
    public AudioClip youWinMusic;

    [Header("Story")]
    public string storySceneName = "Story";

    [Tooltip("Si lo dejas vacío, seguirá usando la música actual o la del menú.")]
    public AudioClip storyMusic;

    [Range(0f, 1f)]
    public float storyVolume = 0.25f;

    public bool useMainMenuMusicInStory = true;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 0.6f;

    [Range(0f, 1f)]
    public float masterVolume = 1f;

    public float fadeDuration = 1f;

    [Header("Save")]
    public string musicVolumePrefsKey = "MusicVolume";

    private AudioSource audioSource;
    private Coroutine fadeRoutine;

    private float currentTargetVolume = 0.6f;

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

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        masterVolume = PlayerPrefs.GetFloat(musicVolumePrefsKey, masterVolume);

        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (sceneName == storySceneName)
        {
            PlayStoryMusic();
            return;
        }

        AudioClip targetClip = null;

        if (sceneName == "Menu" || sceneName == "MainMenu")
            targetClip = mainMenuMusic;
        else if (sceneName == "Game" || sceneName == "GameScene")
            targetClip = gameplayMusic;

        if (targetClip == null)
            return;

        PlayMusic(targetClip, volume);
    }

    private void PlayStoryMusic()
    {
        AudioClip targetClip = null;

        if (storyMusic != null)
            targetClip = storyMusic;
        else if (useMainMenuMusicInStory && mainMenuMusic != null)
            targetClip = mainMenuMusic;

        if (targetClip != null)
        {
            PlayMusic(targetClip, storyVolume);
        }
        else
        {
            FadeToVolume(storyVolume);
        }
    }

    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null)
            PlayMusic(gameOverMusic, volume);
    }

    public void PlayYouWinMusic()
    {
        if (youWinMusic != null)
            PlayMusic(youWinMusic, volume);
    }

    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null)
            PlayMusic(gameplayMusic, volume);
    }

    public void PlayMainMenuMusic()
    {
        if (mainMenuMusic != null)
            PlayMusic(mainMenuMusic, volume);
    }

    private void PlayMusic(AudioClip targetClip, float targetVolume)
    {
        if (targetClip == null)
            return;

        currentTargetVolume = Mathf.Clamp01(targetVolume);

        if (audioSource.clip == targetClip && audioSource.isPlaying)
        {
            FadeToVolume(currentTargetVolume);
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(SwitchMusicRoutine(targetClip, currentTargetVolume));
    }

    private IEnumerator SwitchMusicRoutine(AudioClip newClip, float targetVolume)
    {
        if (audioSource.isPlaying)
            yield return FadeOut();

        audioSource.clip = newClip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return FadeIn(targetVolume);
    }

    private void FadeToVolume(float targetVolume)
    {
        currentTargetVolume = Mathf.Clamp01(targetVolume);

        if (audioSource == null)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (!audioSource.isPlaying)
        {
            audioSource.volume = GetRealVolume(currentTargetVolume);
            return;
        }

        fadeRoutine = StartCoroutine(FadeVolumeRoutine(currentTargetVolume));
    }

    private IEnumerator FadeVolumeRoutine(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float endVolume = GetRealVolume(targetVolume);

        if (fadeDuration <= 0f)
        {
            audioSource.volume = endVolume;
            yield break;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    private IEnumerator FadeIn(float targetVolume)
    {
        float endVolume = GetRealVolume(targetVolume);

        if (fadeDuration <= 0f)
        {
            audioSource.volume = endVolume;
            yield break;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            audioSource.volume = Mathf.Lerp(0f, endVolume, t);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        if (fadeDuration <= 0f)
        {
            audioSource.volume = 0f;
            audioSource.Stop();
            yield break;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private float GetRealVolume(float baseVolume)
    {
        return Mathf.Clamp01(baseVolume * masterVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(musicVolumePrefsKey, masterVolume);
        PlayerPrefs.Save();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.volume = GetRealVolume(currentTargetVolume);
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }
}