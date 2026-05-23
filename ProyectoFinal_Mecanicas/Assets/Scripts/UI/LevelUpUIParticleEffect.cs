using UnityEngine;

public class LevelUpUIParticleEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private ParticleSystem[] particleSystems;

    [Header("Screen Position")]
    [SerializeField] private Vector2 viewportPosition = new Vector2(0.5f, 0.5f);
    [SerializeField] private float distanceFromCamera = 10f;
    [SerializeField] private bool followCameraWhilePlaying = true;

    [Header("Playback")]
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool clearBeforePlay = true;
    [SerializeField] private bool disableObjectWhenStopped = false;

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 80;

    private bool isPlaying;

    private void Awake()
    {
        CacheReferences();
        ConfigureParticles();
        MoveToCameraCenter();
    }

    private void LateUpdate()
    {
        if (!isPlaying)
            return;

        if (followCameraWhilePlaying)
            MoveToCameraCenter();

        if (!AnyParticleSystemAlive())
            isPlaying = false;
    }

    public void Play()
    {
        CacheReferences();

        gameObject.SetActive(true);

        ConfigureParticles();
        MoveToCameraCenter();

        isPlaying = true;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            if (clearBeforePlay)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ps.Play(true);
        }
        Debug.Log("Level Up particles played");
    }

    public void Stop()
    {
        CacheReferences();

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        isPlaying = false;

        if (disableObjectWhenStopped)
            gameObject.SetActive(false);
    }

    private void CacheReferences()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (particleSystems == null || particleSystems.Length == 0)
            particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    private void ConfigureParticles()
    {
        if (particleSystems == null)
            return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            ParticleSystem.MainModule main = ps.main;
            main.useUnscaledTime = useUnscaledTime;

            ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();

            if (renderer != null)
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = sortingOrder;
            }
        }
    }

    private void MoveToCameraCenter()
    {
        if (targetCamera == null)
            return;

        Vector3 viewportPoint = new Vector3(
            viewportPosition.x,
            viewportPosition.y,
            distanceFromCamera
        );

        transform.position = targetCamera.ViewportToWorldPoint(viewportPoint);
        transform.rotation = targetCamera.transform.rotation;
    }

    private bool AnyParticleSystemAlive()
    {
        if (particleSystems == null)
            return false;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null && ps.IsAlive(true))
                return true;
        }

        return false;
    }
}