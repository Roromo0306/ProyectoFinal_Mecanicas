using UnityEngine;
using System.Collections;

public class HitStopService : MonoBehaviour
{
    public static HitStopService Instance;

    private Coroutine routine;

    private void Awake()
    {
        Instance = this;
    }

    public void Stop(float duration, float timeScale = 0.05f)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(HitStopRoutine(duration, timeScale));
    }

    private IEnumerator HitStopRoutine(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.3f * timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        routine = null;
    }
}