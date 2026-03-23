using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthSystem : MonoBehaviour
{
    public int lives = 3;
    public TextMeshProUGUI livesText;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerHitEvent>(OnHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHitEvent>(OnHit);
    }

    private void Start()
    {
        UpdateUI();
    }

    private void OnHit(object evt)
    {
        lives--;
        UpdateUI();

        if (lives <= 0)
            Debug.Log("Game Over");
    }

    private void UpdateUI()
    {
        livesText.text = lives.ToString();
    }
}
