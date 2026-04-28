using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;

    [Header("Settings")]
    public float volume = 0.6f;
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

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
        AudioClip targetClip = null;

        if (sceneName == "Menu")
            targetClip = mainMenuMusic;
        else if (sceneName == "Game")
            targetClip = gameplayMusic;

        if (targetClip == null) return;

        if (audioSource.clip == targetClip && audioSource.isPlaying)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(SwitchMusicRoutine(targetClip));
    }

    private IEnumerator SwitchMusicRoutine(AudioClip newClip)
    {
        yield return FadeOut();

        audioSource.clip = newClip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return FadeIn();
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            audioSource.volume = Mathf.Lerp(0f, volume, t);
            yield return null;
        }

        audioSource.volume = volume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
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
}