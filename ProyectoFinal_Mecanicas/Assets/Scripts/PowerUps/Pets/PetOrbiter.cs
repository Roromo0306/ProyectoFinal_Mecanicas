using UnityEngine;

public class PetOrbiter : MonoBehaviour
{
    private Transform player;
    private PlayerStats playerStats;

    private float currentAngle;
    private int orbitDirection = 1;

    public void Init(Transform playerTransform, PlayerStats stats, float startAngle, bool clockwise)
    {
        player = playerTransform;
        playerStats = stats;
        currentAngle = startAngle;

        // true = horario, false = antihorario
        orbitDirection = clockwise ? -1 : 1;
    }

    private void Update()
    {
        if (player == null || playerStats == null)
            return;

        currentAngle += orbitDirection * playerStats.petOrbitSpeed * Time.deltaTime;

        float radians = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians),
            0f
        ) * playerStats.petOrbitRadius;

        transform.position = player.position + offset;
    }
}