using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    public float smooth = 10f;

    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPos = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smooth * Time.deltaTime
        );
    }
}