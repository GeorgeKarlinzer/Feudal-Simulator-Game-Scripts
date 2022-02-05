using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

public class DirtGeneration : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] dirtTile;
    [Range(0, 1)]
    public float noiseScale;
    [Range(0, 1)]
    public float noiseHeight;

    private Vector2Int mapSize;
    private bool[,] map;


    public void Generate()
    {
        var mapManager = MapManagerStatic;
        mapSize = mapManager.Size;
        map = new bool[mapSize.x, mapSize.y];

        float offset = Random.Range(0, 1000);
        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
            {
                float n = Mathf.PerlinNoise((x + offset) * noiseScale, (y + offset) * noiseScale);
                if (n > noiseHeight && mapManager.GetMapFullness(Vector2Int.one, new Vector2(x, y)))
                    map[x, y] = true;
            }

            SetTiles();
    }

    private void SetTiles()
    {
        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                if (map[x, y])
                {
                    int index = 0;
                    if (y + 1 >= mapSize.y || map[x, y + 1]) index += 1;
                    if (x + 1 >= mapSize.x || map[x + 1, y]) index += 2;
                    if (y - 1 < 0 || map[x, y - 1]) index += 4;
                    if (x - 1 < 0 || map[x - 1, y]) index += 8;
                    tilemap.SetTile(new Vector3Int(x, y, 0), dirtTile[index]);
                }
    }
 
    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
