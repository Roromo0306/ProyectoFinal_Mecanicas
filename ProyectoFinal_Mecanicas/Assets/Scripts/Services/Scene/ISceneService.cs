using UnityEngine.SceneManagement;

public interface ISceneService
{
    void Load(string scene);
    void Quit();
}
