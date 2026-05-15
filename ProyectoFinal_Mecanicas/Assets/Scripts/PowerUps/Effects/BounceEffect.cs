using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Bounce")]
public class BounceEffect : PowerUpEffect
{
    public int amount = 3;
    public float searchRadius = 6f;

    public override void Apply(PlayerStats stats)
    {
        stats.AddBounce(amount, searchRadius);
    }
}