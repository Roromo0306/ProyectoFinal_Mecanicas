using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Dash")]
public class DashEffect : PowerUpEffect
{
    public override void Apply(PlayerContext context)
    {
        context.abilities.UnlockDash();
    }
}