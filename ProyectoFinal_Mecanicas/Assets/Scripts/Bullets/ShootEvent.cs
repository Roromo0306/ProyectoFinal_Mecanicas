using UnityEngine;

public struct ShootEvent
{
    public Vector3 position;
    public Vector3 direction;

    public ShootEvent(Vector3 pos, Vector3 dir)
    {
        position = pos;
        direction = dir;
    }
}
