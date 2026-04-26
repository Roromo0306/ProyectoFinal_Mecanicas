using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Support Pet")]
public class SupportPetEffect : PowerUpEffect
{
    public int extraLives = 2;
    public float speedBonus = 1f;

    public override void Apply(PlayerStats stats)
    {
        stats.hasSupportPet = true;
        stats.maxLives += extraLives;
        stats.moveSpeed += speedBonus;

        Debug.Log("SupportPet -> maxLives: " + stats.maxLives);
    }
}