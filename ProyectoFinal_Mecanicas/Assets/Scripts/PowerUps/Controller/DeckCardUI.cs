using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PowerUpData data;

    public GameObject tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        TooltipUI.Instance.Show(data.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
        TooltipUI.Instance.Hide();
    }

    public void OnClick()
    {
        ActivationService.Instance.Activate(data);
    }
}