using System.Collections;
using UnityEngine;

public class BossLaserAttack : MonoBehaviour
{
    public GameObject laserPrefab;

    public float attackCooldown = 5f;
    public float shakeDuration = 1f;
    public float shakeIntensity = 0.15f;

    public float laserDuration = 3f;
    public float rotationSpeed = 40f;

    public bool IsAttacking { get; private set; }

    private Vector3 originalPosition;
    private FinalBossController bossController;

    private Coroutine attackRoutine;
    private Coroutine performAttackRoutine;

    private GameObject currentLaserGroup;
    private bool isStopping = false;

    private void Start()
    {
        bossController = GetComponent<FinalBossController>();
        attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);

            if (isStopping)
                yield break;

            if (bossController != null && !bossController.CanUseLaserAttack())
                continue;

            performAttackRoutine = StartCoroutine(PerformAttack());
            yield return performAttackRoutine;
            performAttackRoutine = null;
        }
    }

    private IEnumerator PerformAttack()
    {
        if (bossController != null && !bossController.CanUseLaserAttack())
            yield break;

        IsAttacking = true;

        originalPosition = transform.position;

        float timer = 0f;

        while (timer < shakeDuration)
        {
            if (isStopping)
                yield break;

            timer += Time.deltaTime;

            Vector3 randomOffset = Random.insideUnitCircle * shakeIntensity;
            transform.position = originalPosition + randomOffset;

            yield return null;
        }

        transform.position = originalPosition;

        if (bossController != null && !bossController.CanUseLaserAttack())
        {
            IsAttacking = false;
            yield break;
        }

        bool diagonal = Random.value > 0.5f;
        bool rotating = Random.value > 0.5f;

        ClearCurrentLaserGroup();

        currentLaserGroup = new GameObject("LaserGroup");
        currentLaserGroup.transform.position = transform.position;

        float baseAngle = diagonal ? 45f : 0f;

        for (int i = 0; i < 4; i++)
        {
            float angle = baseAngle + i * 90f;

            GameObject laser = Instantiate(
                laserPrefab,
                currentLaserGroup.transform
            );

            laser.transform.localPosition = Vector3.zero;
            laser.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        float elapsed = 0f;

        while (elapsed < laserDuration)
        {
            if (isStopping)
                yield break;

            elapsed += Time.deltaTime;

            if (rotating && currentLaserGroup != null)
            {
                currentLaserGroup.transform.Rotate(
                    Vector3.forward,
                    rotationSpeed * Time.deltaTime
                );
            }

            yield return null;
        }

        ClearCurrentLaserGroup();

        IsAttacking = false;
    }

    public void StopAttackAndClearLasers()
    {
        isStopping = true;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (performAttackRoutine != null)
        {
            StopCoroutine(performAttackRoutine);
            performAttackRoutine = null;
        }

        if (IsAttacking)
            transform.position = originalPosition;

        IsAttacking = false;

        ClearCurrentLaserGroup();
    }

    private void ClearCurrentLaserGroup()
    {
        if (currentLaserGroup != null)
        {
            Destroy(currentLaserGroup);
            currentLaserGroup = null;
        }
    }

    private void OnDestroy()
    {
        ClearCurrentLaserGroup();
    }
}