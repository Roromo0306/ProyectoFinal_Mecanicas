using UnityEngine;

public class DeploymentContinueButton : MonoBehaviour
{
    public UIFlowController flow;

    public void Continue()
    {
        flow.CloseDeployment();
    }
}
