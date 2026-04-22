using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Spread Shot")]
public class SpreadShotEffect : PowerUpEffect
{
    public override void Apply(PlayerStats stats)
    {
        stats.hasSpreadShot = true;
        //Debug.Log("Spread Shot activado");
    }
}