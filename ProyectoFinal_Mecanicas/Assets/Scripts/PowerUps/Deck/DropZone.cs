using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    private CardSlot cardSlot;

    private void Awake()
    {
        cardSlot = GetComponent<CardSlot>();

        if (cardSlot == null)
            cardSlot = GetComponentInParent<CardSlot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (cardSlot != null)
            cardSlot.OnDrop(eventData);
    }
}