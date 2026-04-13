using UnityEngine;
using UnityEngine.EventSystems;

public class DeckCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PowerUpData data;

    public GameObject tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
        
    }

    public void OnClick()
    {
        ActivationService.Instance.Activate(data);
    }
}