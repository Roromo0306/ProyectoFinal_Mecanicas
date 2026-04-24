using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    [SerializeField] private float duration = 900f;
    private float startDuration;

    public float RemainingTime => duration;
    public float ElapsedTime => startDuration - duration;

    private void Awake()
    {
        startDuration = duration;
    }

    private void Update()
    {
        duration -= Time.deltaTime;
        duration = Mathf.Max(duration, 0f);

        int minutes = Mathf.FloorToInt(duration / 60);
        int seconds = Mathf.FloorToInt(duration % 60);

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }
}