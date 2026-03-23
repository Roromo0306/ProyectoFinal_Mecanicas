using UnityEngine;

public class EnemyInstaller : MonoBehaviour
{
    private EnemyController controller;

    private void Awake()
    {
        controller = new EnemyController(transform);
    }

    private void Update()
    {
        controller.Tick();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controller.OnPlayerCollision(collision.transform);
        }
    }
}
