using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    private PowerUpData data;
    private bool alreadySelected = false;

    public void Setup(PowerUpData d)
    {
        data = d;
        alreadySelected = false;

        if (icon != null)
            icon.sprite = d.icon;

        if (title != null)
            title.text = d.title;

        if (desc != null)
            desc.text = d.description;

        CardTooltipTrigger tooltip = GetComponent<CardTooltipTrigger>();
        if (tooltip != null)
            tooltip.SetData(data);
    }

    public void Select()
    {
        if (alreadySelected)
            return;

        alreadySelected = true;

        if (data == null)
        {
            Debug.LogError("LevelUpCardUI.Select -> data es null");
            alreadySelected = false;
            return;
        }

        LevelUpUI levelUpUI = FindObjectOfType<LevelUpUI>();

        if (levelUpUI != null)
        {
            levelUpUI.ChooseCard(data);
            return;
        }

        Debug.LogError("LevelUpCardUI.Select -> No se encontró LevelUpUI");
        alreadySelected = false;
    }
}