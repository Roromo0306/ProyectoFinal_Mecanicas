using UnityEngine;

public class XPCollector : MonoBehaviour
{
    private XPView xpView;
    private bool collected = false;

    private void Awake()
    {
        xpView = GetComponent<XPView>();
    }

    private void OnEnable()
    {
        collected = false;

        if (xpView == null)
            xpView = GetComponent<XPView>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)
            return;

        if (!collision.CompareTag("Player"))
            return;

        collected = true;

        int amount = xpView != null ? xpView.amount : 1;

        EventBus.Publish(new ExperienceCollectedEvent(amount));
        SFXManager.Instance?.PlayXPPickup();

        XPObjectPool.Release(gameObject);
    }
}