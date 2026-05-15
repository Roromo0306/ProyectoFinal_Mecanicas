using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;

    public Canvas fadeCanvas;
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvas != null)
            fadeCanvas.sortingOrder = 9999;

        SetVisible(true);
        SetAlpha(1f);
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isTransitioning = true;

        yield return FadeOut();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
            yield return null;

        yield return null;

        yield return FadeIn();

        isTransitioning = false;
    }

    public IEnumerator FadeIn()
    {
        SetVisible(true);
        yield return Fade(1f, 0f);
        SetAlpha(0f);
        SetVisible(false);
    }

    public IEnumerator FadeOut()
    {
        SetVisible(true);
        yield return Fade(0f, 1f);
        SetAlpha(1f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            SetAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    private void SetVisible(bool visible)
    {
        if (fadeCanvas != null)
            fadeCanvas.enabled = visible;

        if (fadeImage != null)
        {
            fadeImage.enabled = visible;
            fadeImage.raycastTarget = visible;
        }
    }
}