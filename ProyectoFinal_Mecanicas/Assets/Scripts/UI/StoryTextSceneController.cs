using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class StoryTextSceneController : MonoBehaviour
{
    private enum TextEffectType
    {
        Wave,
        Shake
    }

    private class TextEffectRange
    {
        public int startIndex;
        public int endIndex;
        public TextEffectType effectType;
    }

    private class ParsedTextPage
    {
        public string cleanText;
        public List<TextEffectRange> effects = new List<TextEffectRange>();
    }

    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI storyText;

    [TextArea(3, 10)]
    [SerializeField] private string[] textPages;

    [SerializeField] private float timeBetweenCharacters = 0.04f;

    [Header("Sonido al escribir")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingSoundLoop;
    [SerializeField] private float typingVolume = 0.6f;

    [Header("Fade del sonido")]
    [SerializeField] private float soundFadeInDuration = 0.15f;
    [SerializeField] private float soundFadeOutDuration = 0.35f;

    [Header("Randomización del sonido")]
    [SerializeField] private bool startAudioAtRandomPoint = true;

    [Header("Efecto Wave")]
    [SerializeField] private float waveAmplitude = 6f;
    [SerializeField] private float waveSpeed = 8f;
    [SerializeField] private float waveCharacterOffset = 0.35f;

    [Header("Efecto Shake")]
    [SerializeField] private float shakeAmount = 2.5f;
    [SerializeField] private float shakeSpeed = 25f;

    [Header("Cambio de escena")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float delayBeforeNextScene = 0.4f;

    private int currentPageIndex;
    private ParsedTextPage currentParsedPage;

    private Coroutine typingCoroutine;
    private Coroutine fadeSoundCoroutine;

    private bool isTyping;
    private bool isChangingScene;

    private void Start()
    {
        currentPageIndex = 0;

        if (typingAudioSource != null)
        {
            typingAudioSource.loop = true;
            typingAudioSource.playOnAwake = false;
            typingAudioSource.volume = 0f;
        }

        ShowCurrentPage();
    }

    private void Update()
    {
        if (isChangingScene) return;

        if (AdvancePressed())
        {
            if (isTyping)
            {
                CompleteCurrentPageInstantly();
            }
            else
            {
                GoToNextPage();
            }
        }
    }

    private void LateUpdate()
    {
        ApplyTextEffects();
    }

    private bool AdvancePressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Space))
            return true;
#endif

        return false;
    }

    private void ShowCurrentPage()
    {
        if (storyText == null)
        {
            Debug.LogError("Falta asignar Story Text en el inspector.");
            return;
        }

        if (textPages == null || textPages.Length == 0)
        {
            Debug.LogWarning("No hay textos asignados en Text Pages.");
            return;
        }

        currentParsedPage = ParseTextEffects(textPages[currentPageIndex]);

        storyText.text = currentParsedPage.cleanText;
        storyText.maxVisibleCharacters = 0;
        storyText.ForceMeshUpdate();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeCurrentPage());
    }

    private IEnumerator TypeCurrentPage()
    {
        isTyping = true;

        PlayTypingSound();

        storyText.ForceMeshUpdate();

        int totalCharacters = storyText.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            storyText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        isTyping = false;
        FadeOutTypingSound();
    }

    private void CompleteCurrentPageInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        storyText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;

        FadeOutTypingSound();
    }

    private void GoToNextPage()
    {
        currentPageIndex++;

        if (currentPageIndex >= textPages.Length)
        {
            StartCoroutine(GoToNextScene());
            return;
        }

        ShowCurrentPage();
    }

    private IEnumerator GoToNextScene()
    {
        isChangingScene = true;

        FadeOutTypingSound();

        yield return new WaitForSeconds(delayBeforeNextScene);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneFade.Instance.LoadSceneWithFade(nextSceneName);
        }
        else
        {
            int nextBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextBuildIndex);
            }
            else
            {
                Debug.LogWarning("No hay una escena siguiente en Build Settings y Next Scene Name está vacío.");
            }
        }
    }

    private void PlayTypingSound()
    {
        if (typingAudioSource == null || typingSoundLoop == null)
            return;

        if (fadeSoundCoroutine != null)
        {
            StopCoroutine(fadeSoundCoroutine);
            fadeSoundCoroutine = null;
        }

        typingAudioSource.Stop();

        typingAudioSource.clip = typingSoundLoop;
        typingAudioSource.loop = true;
        typingAudioSource.volume = 0f;

        if (startAudioAtRandomPoint && typingSoundLoop.length > 0.05f)
        {
            typingAudioSource.time = Random.Range(0f, typingSoundLoop.length);
        }
        else
        {
            typingAudioSource.time = 0f;
        }

        typingAudioSource.Play();

        fadeSoundCoroutine = StartCoroutine(FadeInSoundCoroutine());
    }

    private void FadeOutTypingSound()
    {
        if (typingAudioSource == null)
            return;

        if (fadeSoundCoroutine != null)
            StopCoroutine(fadeSoundCoroutine);

        fadeSoundCoroutine = StartCoroutine(FadeOutSoundCoroutine());
    }

    private IEnumerator FadeInSoundCoroutine()
    {
        if (soundFadeInDuration <= 0f)
        {
            typingAudioSource.volume = typingVolume;
            yield break;
        }

        float timer = 0f;

        while (timer < soundFadeInDuration)
        {
            timer += Time.deltaTime;
            float t = timer / soundFadeInDuration;

            typingAudioSource.volume = Mathf.Lerp(0f, typingVolume, t);

            yield return null;
        }

        typingAudioSource.volume = typingVolume;
    }

    private IEnumerator FadeOutSoundCoroutine()
    {
        float startVolume = typingAudioSource.volume;

        if (soundFadeOutDuration <= 0f)
        {
            typingAudioSource.Stop();
            typingAudioSource.volume = 0f;
            yield break;
        }

        float timer = 0f;

        while (timer < soundFadeOutDuration)
        {
            timer += Time.deltaTime;
            float t = timer / soundFadeOutDuration;

            typingAudioSource.volume = Mathf.Lerp(startVolume, 0f, t);

            yield return null;
        }

        typingAudioSource.Stop();
        typingAudioSource.volume = 0f;
    }

    private ParsedTextPage ParseTextEffects(string rawText)
    {
        ParsedTextPage parsedPage = new ParsedTextPage();

        StringBuilder cleanBuilder = new StringBuilder();

        int i = 0;

        while (i < rawText.Length)
        {
            if (rawText.Substring(i).StartsWith("<wave>"))
            {
                i += "<wave>".Length;

                int startIndex = cleanBuilder.Length;

                while (i < rawText.Length && !rawText.Substring(i).StartsWith("</wave>"))
                {
                    cleanBuilder.Append(rawText[i]);
                    i++;
                }

                int endIndex = cleanBuilder.Length;

                parsedPage.effects.Add(new TextEffectRange
                {
                    startIndex = startIndex,
                    endIndex = endIndex,
                    effectType = TextEffectType.Wave
                });

                if (i < rawText.Length)
                    i += "</wave>".Length;
            }
            else if (rawText.Substring(i).StartsWith("<shake>"))
            {
                i += "<shake>".Length;

                int startIndex = cleanBuilder.Length;

                while (i < rawText.Length && !rawText.Substring(i).StartsWith("</shake>"))
                {
                    cleanBuilder.Append(rawText[i]);
                    i++;
                }

                int endIndex = cleanBuilder.Length;

                parsedPage.effects.Add(new TextEffectRange
                {
                    startIndex = startIndex,
                    endIndex = endIndex,
                    effectType = TextEffectType.Shake
                });

                if (i < rawText.Length)
                    i += "</shake>".Length;
            }
            else
            {
                cleanBuilder.Append(rawText[i]);
                i++;
            }
        }

        parsedPage.cleanText = cleanBuilder.ToString();

        return parsedPage;
    }

    private void ApplyTextEffects()
    {
        if (storyText == null) return;
        if (currentParsedPage == null) return;
        if (currentParsedPage.effects == null || currentParsedPage.effects.Count == 0) return;

        storyText.ForceMeshUpdate();

        TMP_TextInfo textInfo = storyText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

            if (!characterInfo.isVisible)
                continue;

            if (i >= storyText.maxVisibleCharacters)
                continue;

            Vector3 offset = Vector3.zero;

            foreach (TextEffectRange effect in currentParsedPage.effects)
            {
                int characterStringIndex = characterInfo.index;

                if (characterStringIndex < effect.startIndex || characterStringIndex >= effect.endIndex)
                    continue;

                if (effect.effectType == TextEffectType.Wave)
                {
                    float wave = Mathf.Sin(Time.time * waveSpeed + i * waveCharacterOffset) * waveAmplitude;
                    offset += new Vector3(0f, wave, 0f);
                }
                else if (effect.effectType == TextEffectType.Shake)
                {
                    float x = Mathf.PerlinNoise(i * 10.7f, Time.time * shakeSpeed) - 0.5f;
                    float y = Mathf.PerlinNoise(i * 25.3f, Time.time * shakeSpeed) - 0.5f;

                    offset += new Vector3(x, y, 0f) * shakeAmount * 2f;
                }
            }

            if (offset == Vector3.zero)
                continue;

            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        storyText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}