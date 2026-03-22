using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Subscribe<EnemyHitEvent>(OnHit);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyHitEvent>(OnHit);
    }

    private void OnHit(object evt)
    {
        var e = (EnemyHitEvent)evt;

        if (e.enemy == gameObject)
            Destroy(gameObject);
    }
}
