using UnityEngine;

public class MainMenuBinder : MonoBehaviour
{
    private MainMenuController controller;
    public MainMenuView view;

    private void Awake()
    {
        controller = new MainMenuController();
        controller.Initialize();
    }

    public void Play()
    {
        controller.OnPlay();
    }

    public void Credits()
    {
        controller.OnCredits(view);
    }

    public void Options()
    {
        controller.OnOptions(view);
    }

    public void Exit()
    {
        controller.OnExit();
    }
}