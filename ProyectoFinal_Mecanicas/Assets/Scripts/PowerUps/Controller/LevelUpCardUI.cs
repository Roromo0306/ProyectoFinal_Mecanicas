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

        SFXManager.Instance?.PlayCardSelect();

        if (data == null)
        {
            Debug.LogError("LevelUpCardUI.Select -> data es null");
            return;
        }

        if (SelectionService.Instance == null)
        {
            Debug.LogError("LevelUpCardUI.Select -> SelectionService.Instance es null");
            return;
        }

        SelectionService.Instance.selected = data;
        SelectionService.Instance.AddCardAndAutoEquipIfPossible(data);

        if (ActivationService.Instance != null)
            ActivationService.Instance.RecalculateEquippedPowerUps();

        CloseLevelUpUI();
        OpenDeckUI();
    }

    private void CloseLevelUpUI()
    {
        LevelUpUI levelUpUI = FindObjectOfType<LevelUpUI>();

        if (levelUpUI != null)
            levelUpUI.Hide();
    }

    private void OpenDeckUI()
    {
        if (GameplayDeckMenu.Instance != null)
        {
            GameplayDeckMenu.Instance.OpenDeck();
            return;
        }

        if (UIFlowController.Instance != null)
        {
            UIFlowController.Instance.OpenDeployment();
        }
    }
}