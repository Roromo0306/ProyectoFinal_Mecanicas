using UnityEngine;

public class ExperienceSystemInstaller : MonoBehaviour
{
    private ExperienceController controller;

    private void Awake()
    {
        var model = new ExperienceModel();
        controller = new ExperienceController(model);
    }
}