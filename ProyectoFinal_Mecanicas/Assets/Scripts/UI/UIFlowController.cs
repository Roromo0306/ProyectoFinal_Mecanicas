using UnityEngine;

public class UIFlowController : MonoBehaviour
{
    public static UIFlowController Instance;

    public GameObject levelUpPanel;
    public GameObject deploymentPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenLevelUp()
    {
        levelUpPanel.SetActive(true);
        deploymentPanel.SetActive(false);
    }

    public void OpenDeployment()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        if (deploymentPanel != null)
            deploymentPanel.SetActive(true);

        DeploymentLoader loader = deploymentPanel.GetComponent<DeploymentLoader>();
        if (loader != null)
            loader.Init();

        Time.timeScale = 0f;
    }

    public void CloseDeployment()
    {
        if (deploymentPanel != null)
            deploymentPanel.SetActive(false);

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}