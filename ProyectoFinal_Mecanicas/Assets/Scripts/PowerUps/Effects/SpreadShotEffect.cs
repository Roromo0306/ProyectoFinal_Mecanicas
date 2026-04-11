using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/SpreadShot")]
public class SpreadShotEffect : PowerUpEffect
{
    public override void Apply(PlayerContext context)
    {
        context.weapons.UnlockSpreadShot();
    }
}