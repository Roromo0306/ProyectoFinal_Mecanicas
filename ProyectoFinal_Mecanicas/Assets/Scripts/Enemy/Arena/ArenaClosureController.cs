using UnityEngine;

public class ArenaClosureController : MonoBehaviour
{
    public GameObject topLaser;
    public GameObject bottomLaser;
    public GameObject leftLaser;
    public GameObject rightLaser;

    public Camera mainCamera;
    public float thickness = 0.5f;
    public float inset = 0.5f;

    public Vector3 ArenaCenter { get; private set; }

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        DeactivateArena();
    }

    public void ActivateArena()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        float left = bottomLeft.x + inset;
        float right = topRight.x - inset;
        float bottom = bottomLeft.y + inset;
        float top = topRight.y - inset;

        float width = right - left;
        float height = top - bottom;

        ArenaCenter = new Vector3((left + right) * 0.5f, (bottom + top) * 0.5f, 0f);

        SetupLaser(topLaser, new Vector3(ArenaCenter.x, top, 0f), new Vector3(width, thickness, 1f));
        SetupLaser(bottomLaser, new Vector3(ArenaCenter.x, bottom, 0f), new Vector3(width, thickness, 1f));
        SetupLaser(leftLaser, new Vector3(left, ArenaCenter.y, 0f), new Vector3(thickness, height, 1f));
        SetupLaser(rightLaser, new Vector3(right, ArenaCenter.y, 0f), new Vector3(thickness, height, 1f));

        Debug.Log("Arena activada. Center: " + ArenaCenter);
    }

    public void DeactivateArena()
    {
        SetLaserActive(topLaser, false);
        SetLaserActive(bottomLaser, false);
        SetLaserActive(leftLaser, false);
        SetLaserActive(rightLaser, false);
    }

    private void SetupLaser(GameObject laser, Vector3 position, Vector3 scale)
    {
        if (laser == null)
        {
            Debug.LogError("ArenaClosureController -> falta un laser asignado");
            return;
        }

        laser.transform.position = position;
        laser.transform.localScale = scale;

        BoxCollider2D box = laser.GetComponent<BoxCollider2D>();
        if (box != null)
            box.isTrigger = false;

        ArenaLaserDamage damage = laser.GetComponent<ArenaLaserDamage>();
        if (damage != null)
            damage.SetArenaCenter(ArenaCenter);

        laser.SetActive(true);
    }

    private void SetLaserActive(GameObject laser, bool active)
    {
        if (laser != null)
            laser.SetActive(active);
    }
}