using UnityEngine;
public class CharacterSelectController
{
    private ISceneService sceneService;

    public CharacterSelectController()
    {
        sceneService = ServiceLocator.Get<ISceneService>();
    }

    public void SelectCharacter(int id)
    {
        if (id != 0)
            return;

        PlayerPrefs.SetInt("Character", id);
        sceneService.Load("Bootstrap");
    }
}