using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Support Pet")]
public class SupportPetEffect : PowerUpEffect
{
    public int extraLives = 2;
    public float speedBonus = 1f;

    public float orbitRadius = 2.2f;
    public float orbitSpeed = 160f;

    public override void Apply(PlayerStats stats)
    {
        stats.EnableSupportPet(orbitRadius, orbitSpeed);
        stats.AddMaxLives(extraLives);
        stats.AddMoveSpeed(speedBonus);
    }
}