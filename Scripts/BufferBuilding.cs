using UnityEngine;
using static StaticData;

public class BufferBuilding : BufferObject
{
    [SerializeField] protected BoxCollider2D boxCollider2D;
    [SerializeField] protected LayerMask humanLayer;
    [HideInInspector] public Building buildingPrefab;
    [HideInInspector] public Vector2Int size;

    public override void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float newX = Mathf.RoundToInt(mousePos.x - 0.5f * (size.x % 2 - 1)) + 0.5f * (size.x % 2 - 1);
        float newY = Mathf.RoundToInt(mousePos.y - 0.5f * (size.y % 2 - 1)) + 0.5f * (size.y % 2 - 1);
        float newZ = _transform.position.z;

        _transform.position = new Vector3(newX, newY, newZ);
    }

    public void Rotate()
    {
        int x = size.x;
        size.x = size.y;
        size.y = x;
        _transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + 90));

        Vector3 pos = _transform.position;

        float newX = pos.x;
        float newY = pos.y;
        if (size.x % 2 == 1 && size.y % 2 == 0)
        {
            newX += 0.5f;
            newY -= 0.5f;
        }
        else if (size.x % 2 == 0 && size.y % 2 == 1)
        {
            newX -= 0.5f;
            newY += 0.5f;
        }

        float newZ = _transform.position.z;
        _transform.position = new Vector3(newX, newY, newZ);
    }

    public override void Cancel()
    {
        base.Cancel();

        foreach (var price in buildingPrefab.prices)
            ResManager.res[price.name].SetNormalAmount();
    }

    public override void TryBuild()
    {
        if (!MapManagerStatic.GetMapFullness(size, transform.position) || HumanCheck())
            return;

        var building = Instantiate(buildingPrefab, transform.position, transform.rotation);
        building.transform.position += new Vector3(0, 0, 1);

        if (building.getFreeDoor) BuildMenuStatic.FreeDoors++;
        building.Build();

        base.TryBuild();
    }

    public override void ChangePosition(GameObject movedBuilding)
    {
        if (!MapManagerStatic.GetMapFullness(size, transform.position) || HumanCheck())
            return;

        base.ChangePosition(movedBuilding);

        MapManagerStatic.SetMapFullness(size, movedBuilding.transform.position, false);
    }

    protected bool HumanCheck()
    {
        // Максимальное растояние которое могут пройти челы, до обнавления пути (чтобы чел по инерции не зашел в здание)
        Vector2 borders = new Vector2(0.2f, 0.2f);
        var rayCast = Physics2D.BoxCastAll(_transform.position, boxCollider2D.size + borders, 0, Vector2.zero, 10, humanLayer);
        return rayCast.Length > 0;
    }
}
