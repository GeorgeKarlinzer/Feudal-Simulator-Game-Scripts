using KdTree;
using KdTree.Math;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Forest : MonoBehaviour
{
    [SerializeField] private Tilemap treeTilemap;
    [SerializeField] private Tilemap stumpTilemap;
    [SerializeField] private Tile[] stumpTiles = new Tile[3];
    [SerializeField] private Tile[] treeTiles = new Tile[3];
    public float growingTime;

    public KdTree<float, float> tree = new KdTree<float, float>(2, new FloatMath());

    public void ClearArea(Vector2 pos, Vector2Int size)
    {
        Vector2 firstGrid = pos - (size - Vector2.one) / 2;

        Vector3Int tilemapGrid = new Vector3Int((int)firstGrid.x * ForestGenerator.TPU, (int)firstGrid.y * ForestGenerator.TPU, 0) - new Vector3Int(8, 8, 0);

        for (int x = 0; x < size.x * ForestGenerator.TPU + 6; x++)
            for (int y = 0; y < size.y * ForestGenerator.TPU + 6; y++)
            {
                Vector3Int position = tilemapGrid + new Vector3Int(x, y, 0);
                treeTilemap.SetTile(position, null);
                var worldPos = treeTilemap.CellToWorld(position);
                tree.RemoveAt(new float[] { worldPos.x, worldPos.y });
            }
    }

    public void CutDownTree(Vector3 pos)
    {
        var tilemapPos = treeTilemap.WorldToCell(pos);
        treeTilemap.SetTile(tilemapPos, null);
        stumpTilemap.SetTile(tilemapPos, stumpTiles[Random.Range(0, stumpTiles.Length)]);
        tree.RemoveAt(new float[] { pos.x, pos.y });
        StartCoroutine(TreeGrowing(pos));
    }

    public void AddTree(Vector3 pos)
    {
        tree.Add(new float[] { pos.x, pos.y }, 0);
    }

    public Vector2 GetNearestNeighbours(Transform transform)
    {
        int neighbourLength = 1;
        while (true)
        {
            var a = tree.GetNearestNeighbours(new[] { transform.position.x, transform.position.y }, neighbourLength)[neighbourLength - 1].Point;
            if (MyAstarHandler.IsPathPossible(transform, new Vector3(a[0], a[1], 0)))
            {
                tree.RemoveAt(a);
                return new Vector3(a[0], a[1], 0);
            }

            neighbourLength++;
            if (neighbourLength > tree.Count)
                return Vector3.zero;
        }
    }

    IEnumerator TreeGrowing(Vector3 pos)
    {
        yield return new WaitForSecondsRealtime(growingTime);
        var tilemapPos = treeTilemap.WorldToCell(pos);
        stumpTilemap.SetTile(tilemapPos, null);
        treeTilemap.SetTile(tilemapPos, treeTiles[Random.Range(0, treeTiles.Length)]);
        tree.Add(new float[] { pos.x, pos.y }, 0);
    }
}
