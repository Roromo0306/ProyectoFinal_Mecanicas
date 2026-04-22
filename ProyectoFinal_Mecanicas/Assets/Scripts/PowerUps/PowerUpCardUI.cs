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

        if (icon != null) icon.sprite = data.icon;
        if (title != null) title.text = data.title;
        if (description != null) description.text = data.description;
    }

    public PowerUpData GetData()
    {
        return data;
    }
}