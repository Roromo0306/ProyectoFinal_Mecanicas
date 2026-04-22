using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    public float moveSpeed = 7f;

    private Transform player;
    private PickupSystem pickupSystem;
    private bool attracted = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("XPMagnet -> no se encontró objeto con tag Player");
            return;
        }

        player = playerObj.transform;
        pickupSystem = playerObj.GetComponent<PickupSystem>();

        if (pickupSystem == null)
            Debug.LogError("XPMagnet -> el Player no tiene PickupSystem");
    }

    private void Update()
    {
        if (player == null || pickupSystem == null) return;

        float magnetRadius = pickupSystem.GetMagnetRadius();
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= magnetRadius)
            attracted = true;

        if (attracted)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
    }
}