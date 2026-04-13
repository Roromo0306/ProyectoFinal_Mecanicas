using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelUpCardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    private PowerUpData data;

    public void Setup(PowerUpData d)
    {
        data = d;
        icon.sprite = d.icon;
        title.text = d.title;
        desc.text = d.description;
    }

    public void Select()
    {
        Debug.Log("INSTANCE: " + SelectionService.Instance);
        Debug.Log("DATA: " + data);
        SelectionService.Instance.selected = data;
        Debug.Log(UIFlowController.Instance);
        UIFlowController.Instance.OpenDeployment();
    }
}