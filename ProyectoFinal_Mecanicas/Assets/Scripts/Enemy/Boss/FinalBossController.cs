using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    public float orbitDistance = 6f;
    public float orbitSpeed = 30f;

    public float chaseDistance = 9f;
    public float moveSpeed = 3f;

    private Transform player;
    private float currentAngle;

    private BossLaserAttack laserAttack;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        laserAttack = GetComponent<BossLaserAttack>();

        currentAngle = Random.Range(0f, 360f);
    }

    private void Update()
    {
        if (player == null) return;

        if (laserAttack != null && laserAttack.IsAttacking)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > chaseDistance)
        {
            FollowPlayer();
        }
        else
        {
            OrbitPlayer();
        }
    }

    private void FollowPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
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
}