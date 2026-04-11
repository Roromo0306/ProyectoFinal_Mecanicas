using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    private PowerUpData data;

    public void Setup(PowerUpData data)
    {
        this.data = data;

        icon.sprite = data.icon;
        title.text = data.title;
        description.text = data.description;
    }

    public void OnClick()
    {
        SelectionService.Instance.Select(data);
    }
}