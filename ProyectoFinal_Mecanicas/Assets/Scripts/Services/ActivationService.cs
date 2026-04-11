using UnityEngine;

public class ActivationService : MonoBehaviour
{
    public static ActivationService Instance;

    public PlayerContext playerContext;

    private void Awake()
    {
        Instance = this;
    }

    public void Activate(PowerUpData data)
    {
        foreach (var effect in data.effects)
        {
            effect.Apply(playerContext);
        }
    }
}