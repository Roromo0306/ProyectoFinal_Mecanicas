using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    private PowerUpData data;

    public void Setup(PowerUpData d)
    {
        data = d;

        if (icon != null) icon.sprite = d.icon;
        if (title != null) title.text = d.title;
        if (desc != null) desc.text = d.description;
    }

    public void Select()
    {
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

        UIFlowController.Instance.OpenDeployment();
    }
}