using UnityEngine;

public class LevelUpService : MonoBehaviour
{
    public void LevelUp()
    {
        EventBus.Publish(new LevelUpEvent());
    }
}