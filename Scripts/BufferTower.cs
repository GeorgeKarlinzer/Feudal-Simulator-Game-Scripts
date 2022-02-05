using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

public class BufferTower : BufferBuilding
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile wallTileHorizontal;
    [SerializeField] private Tile wallTileVertical;
    [SerializeField] private LayerMask mask;
    private Tower Tower { get => (Tower)buildingPrefab; }
    private List<Walls> walls = new List<Walls>();

    public override void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float newX = Mathf.RoundToInt(mousePos.x - 0.5f * (size.x % 2 - 1)) + 0.5f * (size.x % 2 - 1);
        float newY = Mathf.RoundToInt(mousePos.y - 0.5f * (size.y % 2 - 1)) + 0.5f * (size.y % 2 - 1);
        float newZ = _transform.position.z;

        _transform.position = new Vector3(newX, newY, newZ);

        if (walls.Count == 0 || walls[0].position != _transform.position)
        {
            tilemap.ClearAllTiles();
            walls = new List<Walls>();

            foreach (var p in Tower.towerPos)
            {
                if (p.x == newX)
                    ShowWalls(p, new Vector3Int(0, Sign(p.y - newY), 0), (int)Mathf.Abs(p.y - newY));
                if (p.y == newY)
                    ShowWalls(p, new Vector3Int(Sign(p.x - newX), 0, 0), (int)Mathf.Abs(p.x - newX));
            }
        }
    }

    private void OnDisable()
    {
        var price = GetPrice();
        foreach (var p in price)
            ResManager.res[p.name].SetPossibleAmount(p.amount);

    }

    public override void TryBuild()
    {
        if (!MapManagerStatic.GetMapFullness(size, _transform.position) || !Price.CanBuy(GetPrice().ToArray()) || HumanCheck())
            return;

        foreach (var wall in walls)
        {
            var v = wall.position - wall.targetPosition;
            if (Mathf.Abs(v.x + v.y) == 1)
                return;
        }

        var building = Instantiate(Tower, _transform.position, _transform.rotation);
        building.transform.position += new Vector3(0, 0, 1);
        building.walls = new List<Walls>(walls);
        building.Build();

        tilemap.ClearAllTiles();
        Cancel();
    }

    private List<Price> GetPrice()
    {
        var price = new List<Price>(Tower.prices);
        int wallsCount = 0;
        for (int i = 0; i < walls.Count; i++)
        {
            Vector3 pos = walls[i].position;
            Vector3 targetPos = walls[i].targetPosition;
            int count = pos.x == targetPos.x ? (int)(pos.y - targetPos.y) : (int)(pos.x - targetPos.x);
            if (count == 0) count = 1;
            wallsCount += Mathf.Abs(count) - 1;

        }
        foreach (var item in Tower.WallPrice)
        {
            bool flag = false;
            for (int i = 0; i < price.Count; i++)
            {
                if (price[i].name == item.name)
                {
                    int amount = price[i].amount + item.amount * wallsCount;
                    price[i] = new Price(item.name, amount);
                    flag = true;
                    break;
                }
            }
            if (!flag)
                price.Add(new Price(item.name, item.amount * wallsCount));
        }

        return price;
    }

    public override void Cancel()
    {
        base.Cancel();
        var price = GetPrice();
        foreach (var p in price)
            ResManager.res[p.name].SetNormalAmount();

        tilemap.ClearAllTiles();
        walls = new List<Walls>();
    }

    private void ShowWalls(Vector3 finalPos, Vector3Int step, int stepsCount)
    {
        Vector3Int[] positions = new Vector3Int[stepsCount];
        Tile[] tiles = new Tile[stepsCount];

        for (int i = 1; i <= stepsCount; i++)
        {
            var pos2 = finalPos - step * i;
            var pos3 = new Vector3Int((int)pos2.x, (int)pos2.y, 0);

            if (!MapManagerStatic.GetMapFullnessInGrid((Vector2Int)pos3))
                return;

            positions[i - 1] = pos3;

            tiles[i - 1] = step.x == 0 ? wallTileVertical : wallTileHorizontal;

        }

        walls.Add(new Walls(_transform.position, finalPos, new Vector3Int(step.x, step.y, 0)));
        tilemap.SetTiles(positions, tiles);
    }

    private int Sign(float value) => value > 0 ? 1 : value < 0 ? -1 : 0;
}

public struct Walls
{
    public Walls(Vector3 position, Vector3 targetPosition, Vector3Int step)
    {
        this.position = position;
        this.targetPosition = targetPosition;
        this.step = step;
    }
    public Vector3 position;
    public Vector3 targetPosition;
    public Vector3Int step;
}
