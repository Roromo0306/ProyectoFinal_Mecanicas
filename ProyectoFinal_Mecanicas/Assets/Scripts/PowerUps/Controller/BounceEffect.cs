using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Bounce")]
public class BounceEffect : PowerUpEffect
{
    public int amount = 3;

    public override void Apply(PlayerStats stats)
    {
        stats.bounceCount += amount;
        //Debug.Log("Bounce activado -> " + stats.bounceCount);
    }
}