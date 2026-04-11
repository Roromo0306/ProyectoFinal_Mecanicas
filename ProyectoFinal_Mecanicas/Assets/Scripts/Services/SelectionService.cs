using UnityEngine;

public class SelectionService : MonoBehaviour
{
    public static SelectionService Instance;

    public PowerUpData selected;

    private void Awake()
    {
        Instance = this;
    }

    public void Select(PowerUpData data)
    {
        selected = data;

        Time.timeScale = 1f;

        UIManager.Instance.OpenDeploymentScreen(data);
    }
}
