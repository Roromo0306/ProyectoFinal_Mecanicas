using UnityEngine;

public class SelectionService : MonoBehaviour
{
    public static SelectionService Instance;

    public PowerUpData selected;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}