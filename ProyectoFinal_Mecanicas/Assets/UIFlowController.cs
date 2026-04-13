using UnityEngine;

public class UIFlowController : MonoBehaviour
{
    public GameObject levelUpPanel;
    public GameObject deploymentPanel;

    public static UIFlowController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenLevelUp()
    {
        levelUpPanel.SetActive(true);
        deploymentPanel.SetActive(false);
    }

    public void OpenDeployment()
    {
        var dp = deploymentPanel;
        var lp = levelUpPanel;

        if (!dp || !lp)
        {
            Debug.LogError("REFERENCIAS ROTAS EN RUNTIME");
            return;
        }

        lp.SetActive(false);
        dp.SetActive(true);

        var loader = dp.GetComponent<DeploymentLoader>();
        if (!loader)
        {
            Debug.LogError("DeploymentLoader NO está en el panel activo");
            return;
        }

        loader.Init();
    }

    public void CloseDeployment()
    {
        deploymentPanel.SetActive(false);
        levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}