using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSliderUI : MonoBehaviour
{
    [Header("UI")]
    public Slider volumeSlider;

    private void Start()
    {
        if (volumeSlider == null)
            volumeSlider = GetComponent<Slider>();

        if (volumeSlider == null)
        {
            Debug.LogWarning("No hay Slider asignado al MusicVolumeSliderUI.");
            return;
        }

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.wholeNumbers = false;

        float savedVolume = 1f;

        if (MusicManager.Instance != null)
            savedVolume = MusicManager.Instance.GetMasterVolume();
        else if (SFXManager.Instance != null)
            savedVolume = SFXManager.Instance.GetMasterVolume();

        volumeSlider.value = savedVolume;

        ApplyVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(ApplyVolume);
    }

    private void ApplyVolume(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetMasterVolume(value);

        if (SFXManager.Instance != null)
            SFXManager.Instance.SetMasterVolume(value);
    }
}