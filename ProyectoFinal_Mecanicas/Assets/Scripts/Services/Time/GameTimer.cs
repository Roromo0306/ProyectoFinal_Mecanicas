using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float duration = 900f;

    private void Update()
    {
        duration -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(duration / 60);
        int seconds = Mathf.FloorToInt(duration % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}