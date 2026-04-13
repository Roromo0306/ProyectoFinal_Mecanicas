using UnityEngine;

public class DeploymentLoader : MonoBehaviour
{
    public Transform deckParent;
    public GameObject cardPrefab;

    public void Init()
    {
        LoadCard();
    }

    void LoadCard()
    {
        Debug.Log("Instance: " + SelectionService.Instance);
        Debug.Log("Selected: " + SelectionService.Instance?.selected);
        Debug.Log("Prefab: " + cardPrefab);
        Debug.Log("Parent: " + deckParent);

        var data = SelectionService.Instance.selected;

        var card = Instantiate(cardPrefab, deckParent);
        Debug.Log("SELECTED: " + SelectionService.Instance.selected);
        card.GetComponent<DragCard>().data = data;

        Debug.Log("DEPLOYMENT LOAD");
        Debug.Log("Selected: " + SelectionService.Instance.selected);
        Debug.Log("Prefab: " + cardPrefab);
        Debug.Log("Parent: " + deckParent);
    }
}