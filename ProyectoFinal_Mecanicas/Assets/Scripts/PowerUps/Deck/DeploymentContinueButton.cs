using UnityEngine;

public class DeploymentContinueButton : MonoBehaviour
{
    public UIFlowController flow;

    public void Continue()
    {
        if (flow == null)
        {
            Debug.LogError("DeploymentContinueButton -> flow es null");
            return;
        }

        flow.CloseDeployment();
    }
}
