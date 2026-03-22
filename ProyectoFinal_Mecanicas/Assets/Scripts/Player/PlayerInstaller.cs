using UnityEngine;

public class PlayerInstaller : MonoBehaviour
{
    private PlayerController controller;

    private void Awake()
    {
        var model = new PlayerModel();
        var view = GetComponent<PlayerView>();

        controller = new PlayerController(model, view);
    }

    private void Update()
    {
        controller.Tick();
    }
}