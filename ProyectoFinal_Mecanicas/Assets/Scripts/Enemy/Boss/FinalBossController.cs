using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    [Header("Orbit")]
    public float orbitDistance = 6f;
    public float orbitSpeed = 30f;

    [Header("Movement")]
    public float chaseDistance = 9f;
    public float moveSpeed = 3f;

    [Header("Attack Positioning")]
    public float maxAttackDistance = 7f;
    public float minAttackDistance = 4f;
    public bool isReadyToAttack = false;

    private Transform player;
    private float currentAngle;

    private BossLaserAttack laserAttack;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        laserAttack = GetComponent<BossLaserAttack>();

        currentAngle = Random.Range(0f, 360f);
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si está atacando, no moverlo.
        if (laserAttack != null && laserAttack.IsAttacking)
            return;

        // Solo está listo para atacar si está cerca del player.
        isReadyToAttack = distanceToPlayer <= maxAttackDistance;

        // Si está demasiado lejos, acercarse antes de atacar.
        if (distanceToPlayer > maxAttackDistance)
        {
            FollowPlayer();
            return;
        }

        // Si está demasiado cerca, se separa un poco.
        if (distanceToPlayer < minAttackDistance)
        {
            MoveAwayFromPlayer();
            return;
        }

        // Si está en buena distancia, orbita.
        OrbitPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void OrbitPlayer()
    {
        currentAngle += orbitSpeed * Time.deltaTime;

        float radians = currentAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(radians),
            Mathf.Sin(radians),
            0f
        ) * orbitDistance;

        Vector3 targetPos = player.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * 2f
        );
    }

    public bool CanUseLaserAttack()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer <= maxAttackDistance && distanceToPlayer >= minAttackDistance;
    }
}