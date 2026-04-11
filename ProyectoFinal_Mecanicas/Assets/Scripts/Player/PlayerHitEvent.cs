using UnityEngine;
public struct PlayerHitEvent {
    public Vector3 hitPosition;

    public PlayerHitEvent(Vector3 hitPosition)
    {
        this.hitPosition = hitPosition;
    }
}
