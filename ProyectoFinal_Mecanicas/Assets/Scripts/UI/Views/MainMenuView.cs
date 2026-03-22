using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject optionsPanel;

    public void ShowCredits(bool state)
    {
        creditsPanel.SetActive(state);
    }

    public void ShowOptions(bool state)
    {
        optionsPanel.SetActive(state);
    }
}