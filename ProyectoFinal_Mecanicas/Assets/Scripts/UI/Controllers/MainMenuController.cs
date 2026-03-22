public class MainMenuController
{
    private ISceneService sceneService;

    public MainMenuController()
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