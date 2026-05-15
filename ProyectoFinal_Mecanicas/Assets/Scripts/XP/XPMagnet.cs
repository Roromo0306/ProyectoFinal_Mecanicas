using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;

    private static Transform cachedPlayer;

    private Transform player;
    private PickupSystem pickupSystem;
    private bool attracted = false;

    private void OnEnable()
    {
        attracted = false;
        TryCachePlayer();
    }

    private void Start()
    {
        TryCachePlayer();
    }

    private void Update()
    {
        if (player == null || pickupSystem == null)
            TryCachePlayer();

        if (player == null || pickupSystem == null)
            return;

        float magnetRadius = pickupSystem.GetMagnetRadius();
        float sqrDistance = (transform.position - player.position).sqrMagnitude;

        if (sqrDistance <= magnetRadius * magnetRadius)
            attracted = true;

        if (!attracted)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void TryCachePlayer()
    {
        if (cachedPlayer == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
                return;

            cachedPlayer = playerObject.transform;
        }

        player = cachedPlayer;
        pickupSystem = player.GetComponent<PickupSystem>();
    }
}