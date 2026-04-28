using TMPro;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(TMP_Text))]
public class TMPWaveText : MonoBehaviour
{
    public float waveSpeed = 4f;
    public float waveHeight = 4f;
    public float waveFrequency = 0.35f;

    private TMP_Text text;

    private List<int> waveCharIndices = new List<int>();

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void SetText(string rawText)
    {
        waveCharIndices.Clear();

        string cleanText = "";
        bool inWave = false;
        int visibleIndex = 0;

        for (int i = 0; i < rawText.Length; i++)
        {
            // 🔥 ABRIR WAVE
            if (i <= rawText.Length - 6 && rawText.Substring(i, 6) == "<wave>")
            {
                inWave = true;
                i += 5;
                continue;
            }

            // 🔥 CERRAR WAVE (IMPORTANTE: 7 caracteres)
            if (i <= rawText.Length - 7 && rawText.Substring(i, 7) == "</wave>")
            {
                inWave = false;
                i += 6;
                continue;
            }

            // 🔥 DETECTAR TAG TMP (<color>, etc.)
            if (rawText[i] == '<')
            {
                while (i < rawText.Length && rawText[i] != '>')
                {
                    cleanText += rawText[i];
                    i++;
                }

                if (i < rawText.Length)
                    cleanText += rawText[i];

                continue;
            }

            // 🔥 CARÁCTER NORMAL
            cleanText += rawText[i];

            if (inWave)
                waveCharIndices.Add(visibleIndex);

            visibleIndex++;
        }

        text.text = cleanText;
    }

    private void Update()
    {
        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;

        foreach (int i in waveCharIndices)
        {
            if (i >= textInfo.characterCount) continue;
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float offset = Mathf.Sin(Time.unscaledTime * waveSpeed + i * waveFrequency) * waveHeight;

            for (int j = 0; j < 4; j++)
                vertices[vertexIndex + j].y += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}