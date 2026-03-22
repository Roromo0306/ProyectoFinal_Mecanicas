using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : ISceneService
{
    public void Load(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}