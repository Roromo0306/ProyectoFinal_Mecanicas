using UnityEngine;
using System.Collections;

public class CameraShakeService : MonoBehaviour
{
    public static CameraShakeService Instance;

    private Transform cam;
    private Vector3 originalPos;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main.transform;
        originalPos = cam.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        cam.localPosition = originalPos;
    }
}