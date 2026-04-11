using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject panel;
    public TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string content)
    {
        panel.SetActive(true);
        text.text = content;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}