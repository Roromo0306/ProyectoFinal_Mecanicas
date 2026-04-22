using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DeckCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PowerUpData data;
    public GameObject tooltip;

    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void Setup(PowerUpData newData)
    {
        data = newData;

        if (icon != null) icon.sprite = data.icon;
        if (title != null) title.text = data.title;
        if (description != null) description.text = data.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.SetActive(false);
    }
}