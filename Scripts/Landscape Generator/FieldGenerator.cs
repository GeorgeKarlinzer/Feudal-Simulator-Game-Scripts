using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile grassTile;

    public void Generate()
    {
        var startPos = MapManagerStatic.firstCoord;
        var mapSize = MapManagerStatic.Size;
        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                tilemap.SetTile(new Vector3Int(startPos.x + x, startPos.y + y, 0), grassTile);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
