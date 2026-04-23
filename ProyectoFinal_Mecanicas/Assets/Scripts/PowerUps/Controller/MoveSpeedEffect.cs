using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Move Speed")]
public class MoveSpeedEffect : PowerUpEffect
{
    public float speedBonus = 1f;

    public override void Apply(PlayerStats stats)
    {
        stats.moveSpeed += speedBonus;
        Debug.Log("Velocidad aumentada -> " + stats.moveSpeed);
    }
}