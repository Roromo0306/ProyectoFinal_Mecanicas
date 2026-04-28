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

        if (icon != null) icon.sprite = d.icon;
        if (title != null) title.text = d.title;
        if (desc != null) desc.text = d.description;
        CardTooltipTrigger tooltip = GetComponent<CardTooltipTrigger>();
        if (tooltip != null)
            tooltip.SetData(data);
    }

    public void Select()
    {
        if (alreadySelected) return;
        alreadySelected = true;

        if (data == null)
        {
            Debug.LogError("LevelUpCardUI.Select -> data es null");
            return;
        }

        if (SelectionService.Instance == null)
        {
            Debug.LogError("SelectionService.Instance es null");
            return;
        }

        SelectionService.Instance.selected = data;
        SelectionService.Instance.AddToDeck(data);

        Debug.Log("Seleccionada -> " + data.title);

        LevelUpUI levelUpUI = FindObjectOfType<LevelUpUI>();
        if (levelUpUI != null)
            levelUpUI.MarkClosed();

        if (GameplayDeckMenu.Instance != null)
        {
            GameplayDeckMenu.Instance.OpenDeck();
        }
        else if (UIFlowController.Instance != null)
        {
            UIFlowController.Instance.OpenDeployment();
        }
    }
}