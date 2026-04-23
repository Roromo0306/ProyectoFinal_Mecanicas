using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Support Pet")]
public class SupportPetEffect : PowerUpEffect
{
    public int extraLives = 2;
    public float speedBonus = 1f;

    public override void Apply(PlayerStats stats)
    {
        if (stats.hasSupportPet)
        {
            Debug.Log("La mascota support ya estaba activa");
            return;
        }

        stats.hasSupportPet = true;
        stats.maxLives += extraLives;
        stats.moveSpeed += speedBonus;

        PlayerHealthSystem health = Object.FindObjectOfType<PlayerHealthSystem>();
        if (health != null)
        {
            health.AddLives(extraLives);
            Debug.Log("Mascota support -> vidas añadidas: " + extraLives);
        }
        else
        {
            Debug.LogError("SupportPetEffect -> no se encontró PlayerHealthSystem");
        }

        Debug.Log("Mascota support activada | maxLives: " + stats.maxLives + " | moveSpeed: " + stats.moveSpeed);
    }
}