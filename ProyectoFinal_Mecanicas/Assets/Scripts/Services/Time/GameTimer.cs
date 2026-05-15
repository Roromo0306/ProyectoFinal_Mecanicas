using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    [SerializeField] private float duration = 900f;

    private float startDuration;
    private bool victoryTriggered = false;

    public float RemainingTime => duration;
    public float ElapsedTime => startDuration - duration;

    private void Awake()
    {
        startDuration = duration;
        EnemyXPDropper.ResetRunTimer();
    }

    private void Update()
    {
        if (victoryTriggered)
            return;

        duration -= Time.deltaTime;
        duration = Mathf.Max(duration, 0f);

        UpdateTimerUI();

        if (duration <= 0f)
            TriggerVictory();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(duration / 60);
        int seconds = Mathf.FloorToInt(duration % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void TriggerVictory()
    {
        victoryTriggered = true;

        if (EndGameUI.Instance != null)
            EndGameUI.Instance.ShowWin();
    }
}