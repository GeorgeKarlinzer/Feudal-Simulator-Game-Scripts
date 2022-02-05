using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

public class BorderGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile borderTile;

    public void Generate()
    {
        MapManager map = MapManagerStatic;

        map.SetMapFullness(new Vector2Int(1, map.lastCoord.y - map.firstCoord.y), new Vector2(map.firstCoord.x, (map.lastCoord.y - map.firstCoord.y) / 2f), false);
        map.SetMapFullness(new Vector2Int(1, map.lastCoord.y - map.firstCoord.y), new Vector2(map.lastCoord.x - 1, (map.lastCoord.y - map.firstCoord.y) / 2f), false);

        map.SetMapFullness(new Vector2Int(map.lastCoord.x - map.firstCoord.x, 1), new Vector2((map.lastCoord.x - map.firstCoord.x) / 2f, map.firstCoord.y), false);
        map.SetMapFullness(new Vector2Int(map.lastCoord.x - map.firstCoord.x, 1), new Vector2((map.lastCoord.x - map.firstCoord.x) / 2f, map.lastCoord.y - 1), false);

        var startPos = map.firstCoord;
        for (int x = 0; x < map.Size.x; x++)
        {
            tilemap.SetTile(new Vector3Int(startPos.x + x, map.firstCoord.y, 0), borderTile);
            tilemap.SetTile(new Vector3Int(startPos.x + x, map.lastCoord.y - 1, 0), borderTile);

        }
        for (int y = 0; y < map.Size.y; y++)
        {
            tilemap.SetTile(new Vector3Int(map.firstCoord.x, startPos.y + y, 0), borderTile);
            tilemap.SetTile(new Vector3Int(map.lastCoord.x - 1, startPos.y + y, 0), borderTile);
        }
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
