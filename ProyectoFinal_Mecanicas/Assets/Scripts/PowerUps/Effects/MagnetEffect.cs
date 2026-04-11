using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Magnet")]
public class MagnetEffect : PowerUpEffect
{
    public float increase = 2f;

    public override void Apply(PlayerContext context)
    {
        context.pickups.magnetRadius += increase;
    }
}