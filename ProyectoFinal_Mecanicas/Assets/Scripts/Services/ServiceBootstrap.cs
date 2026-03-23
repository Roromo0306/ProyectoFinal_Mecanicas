using UnityEngine;

public static class ServicesBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        ServiceLocator.Register<IInputService>(new InputService());
        ServiceLocator.Register<ISceneService>(new SceneService());
        ServiceLocator.Register<ITimeService>(new TimeService());
        Debug.Log("Servicios inicializados antes de cargar escenas");
    }
}
