public class MainMenuController
{

    private ISceneService sceneService;

    // Constructor limpio, no toca servicios todavía
    public MainMenuController()
    {
        // No usar ServiceLocator aquí
    }

    // Nuevo método: Initialize
    public void Initialize()
    {
        sceneService = ServiceLocator.Get<ISceneService>();
    }

    public void OnPlay()
    {
        sceneService.Load("CharacterSelect");
    }

    public void OnCredits(MainMenuView view)
    {
        view.ShowCredits(true);
    }

    public void OnOptions(MainMenuView view)
    {
        view.ShowOptions(true);
    }

    public void OnExit()
    {
        sceneService.Quit();
    }
}