using UnityEngine;

public class GlobalServices : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        ServiceLocator.Register<IInputService>(new InputService());
        ServiceLocator.Register<ISceneService>(new SceneService());
        ServiceLocator.Register<ITimeService>(new TimeService());
    }
}