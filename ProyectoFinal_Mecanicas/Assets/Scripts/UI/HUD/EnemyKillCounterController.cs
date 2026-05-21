using UnityEngine;

public class EnemyKillCounterController : MonoBehaviour
{
    [SerializeField] private bool resetOnEnable = true;

    private readonly EnemyKillCounterModel model = new EnemyKillCounterModel();

    private void OnEnable()
    {
        if (resetOnEnable)
            model.Reset();

        EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        PublishCurrentCount();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
    }

    private void OnEnemyKilled(EnemyKilledEvent e)
    {
        model.AddKill();
        PublishCurrentCount();
    }

    private void PublishCurrentCount()
    {
        EventBus.Publish(new EnemyKillCountUpdatedEvent(model.TotalKills));
    }
}
