using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register<IInputService>(new InputService());
        ServiceLocator.Register<ISceneService>(new SceneService());

        ServiceLocator.Register<ITimeService>(new TimeService());
    }

    private void Start()
    {
        ServiceLocator.Get<ISceneService>().Load("Game");
    }
}
