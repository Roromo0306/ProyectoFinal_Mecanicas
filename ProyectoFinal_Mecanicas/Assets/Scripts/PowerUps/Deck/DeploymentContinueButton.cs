using UnityEngine;

public class DeploymentContinueButton : MonoBehaviour
{
    public UIFlowController flow;

    public void Continue()
    {
        if (GameplayDeckMenu.Instance != null)
        {
            GameplayDeckMenu.Instance.CloseDeck();
            return;
        }

        if (flow != null)
        {
            flow.CloseDeployment();
            return;
        }

        Debug.LogError("DeploymentContinueButton -> No hay GameplayDeckMenu ni UIFlowController asignado");
        Time.timeScale = 1f;
    }
}