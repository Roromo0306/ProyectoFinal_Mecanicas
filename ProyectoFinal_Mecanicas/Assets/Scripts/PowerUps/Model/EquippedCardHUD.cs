using UnityEngine;
using UnityEngine.UI;

public class EquippedCardsHUD : MonoBehaviour
{
    public static EquippedCardsHUD Instance;

    [Header("Icon Placers")]
    public Image[] iconPlacers = new Image[6];

    [Header("Empty Slot")]
    public Sprite emptyIcon;
    public bool hideEmptyIcons = true;

    private void Awake()
    {
        Instance = this;
        Refresh();
    }

    public void Refresh()
    {
        if (SelectionService.Instance == null)
            return;

        PowerUpData[] equippedSlots = SelectionService.Instance.equippedSlots;

        for (int i = 0; i < iconPlacers.Length; i++)
        {
            Image iconImage = iconPlacers[i];

            if (iconImage == null)
                continue;

            PowerUpData card = null;

            if (equippedSlots != null && i < equippedSlots.Length)
                card = equippedSlots[i];

            if (card != null)
            {
                Sprite spriteToUse = card.hudIcon != null ? card.hudIcon : card.icon;

                iconImage.sprite = spriteToUse;
                iconImage.enabled = true;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.sprite = emptyIcon;

                if (hideEmptyIcons)
                    iconImage.enabled = false;
                else
                    iconImage.enabled = true;
            }
        }
    }
}