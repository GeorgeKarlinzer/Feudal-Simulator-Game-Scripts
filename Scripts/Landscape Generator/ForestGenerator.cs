using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

public class ForestGenerator : MonoBehaviour
{
    [SerializeField] private Forest forest;

    public Tilemap tilemap;
    public Tile[] treeTile;

    [Range(0, 0.2f)]
    public float treeChance;
    public float noiseScale;
    [Range(0, 1f)]
    public float noiseHeight;

    /// <summary>
    /// Tiles per unit
    /// </summary>
    public const int TPU = 10;
    private MapManager mapManager;
    private Vector2Int mapSize;

    public void Clear()
    {
        tilemap.ClearAllTiles();
        forest.tree.Clear();
    }

    public void Generate()
    {
        mapManager = MapManagerStatic;
        mapSize = mapManager.Size;

        tilemap.SetTile(new Vector3Int(0, 0, 0), treeTile[0]);

        float offset = Random.Range(1000, 2000);
        for (int x = 4; x < (mapSize.x - 2) * TPU + 9; x++)
            for (int y = 4; y < (mapSize.y - 2) * TPU + 9; y++)
            {
                float n = Mathf.PerlinNoise((x + offset) * noiseScale, (y + offset) * noiseScale);
                var worldPos = tilemap.CellToWorld(new Vector3Int(x, y, 0));
                if (n > noiseHeight && mapManager.GetMapFullness(Vector2Int.one * 3, new Vector3(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y))))
                {
                    if (Random.value < treeChance)
                    {
                        var pos = new Vector3Int(x, y, 0);
                        tilemap.SetTile(pos, treeTile[Random.Range(0, treeTile.Length - 1)]);
                        forest.tree.Add(new float[] { worldPos.x, worldPos.y }, 0);
                    }
                }
            }
    }
}
