using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    [SerializeField] private Building wallPrefab;
    public List<Price> WallPrice { get => wallPrefab.prices; }
    public static List<Vector3> towerPos = new List<Vector3>();

    public List<Walls> walls = new List<Walls>();

    public override void Build()
    {
        base.Build();
        towerPos.Add(transform.position);
        BuildWalls();
    }

    private void BuildWalls()
    {
        foreach (var wall in walls)
        {
            for (var pos = (Vector2)(wall.targetPosition - wall.step); pos != (Vector2)wall.position; pos -= (Vector2Int)wall.step)
            {
                var building = Instantiate(wallPrefab, pos, wall.step.x == 0 ? Quaternion.Euler(0, 0, 90) : Quaternion.identity);
                building.Build();
            }
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        towerPos.Remove(transform.position);
    }
}
