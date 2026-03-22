using UnityEngine;

public interface IInputService
{
    Vector2 GetMovement();
    Vector3 GetMouseWorldPosition();
    bool GetFire();
}