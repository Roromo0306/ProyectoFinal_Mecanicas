using UnityEngine;

public class PlayerInstaller : MonoBehaviour
{
    private PlayerController controller;

    private void Awake()
    {
        var model = new PlayerModel();
        var view = GetComponent<PlayerView>();

        if (view == null)
        {
            Debug.LogError("PlayerInstaller -> falta PlayerView en " + gameObject.name);
            return;
        }

        controller = new PlayerController(model, view);
    }

    private void Update()
    {
        if (controller == null) return;
        controller.Tick();
    }
}