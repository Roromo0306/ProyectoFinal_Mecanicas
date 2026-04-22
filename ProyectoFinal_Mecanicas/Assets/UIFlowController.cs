using UnityEngine;

public class UIFlowController : MonoBehaviour
{
    public GameObject levelUpPanel;
    public GameObject deploymentPanel;

    public static UIFlowController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OpenLevelUp()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(true);
        if (deploymentPanel != null) deploymentPanel.SetActive(false);
    }

    public void OpenDeployment()
    {
        if (deploymentPanel == null || levelUpPanel == null)
        {
            Debug.LogError("REFERENCIAS ROTAS EN RUNTIME");
            return;
        }

        levelUpPanel.SetActive(false);
        deploymentPanel.SetActive(true);

        var loader = deploymentPanel.GetComponent<DeploymentLoader>();
        if (loader == null)
        {
            Debug.LogError("DeploymentLoader no está en deploymentPanel");
            return;
        }

        loader.Init();
    }

    public void CloseDeployment()
    {
        if (deploymentPanel != null) deploymentPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        if (SelectionService.Instance != null)
            SelectionService.Instance.ClearSelection();

        Time.timeScale = 1f;
    }
}