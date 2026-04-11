using UnityEngine;
using System.Collections.Generic;

public class LevelUpUIController : MonoBehaviour
{
    public GameObject panel;
    public List<PowerUpCardUI> cards;

    private void OnEnable()
    {
        LevelUpService.Instance.OnLevelUp += Show;
    }

    private void OnDisable()
    {
        LevelUpService.Instance.OnLevelUp -= Show;
    }

    private void Show(List<PowerUpData> options)
    {
        panel.SetActive(true);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Setup(options[i]);
        }
    }
}