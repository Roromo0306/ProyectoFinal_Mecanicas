using UnityEngine;

public class XPCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger con: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("XP recogida");

            var xp = GetComponent<XPView>();
            EventBus.Publish(new ExperienceCollectedEvent(xp.amount));
            SFXManager.Instance?.PlayXPPickup();

            Destroy(gameObject);
        }
    }
}