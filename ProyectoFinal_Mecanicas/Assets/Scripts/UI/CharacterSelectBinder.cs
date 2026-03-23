using UnityEngine;

public class CharacterSelectBinder : MonoBehaviour
{
    private CharacterSelectController controller;

    private void Awake()
    {
        controller = new CharacterSelectController();
    }

    public void SelectCharacter(int id)
    {
        controller.SelectCharacter(id);
    }
}