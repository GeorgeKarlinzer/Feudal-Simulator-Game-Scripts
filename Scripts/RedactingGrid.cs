using UnityEngine;
using UnityEngine.Tilemaps;

public class RedactingGrid : MonoBehaviour
{
    [SerializeField] private MapManager map;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile redTile;
    [SerializeField] private Tile greenTile;

    private void Awake()
    {
        CreateGrid();
        HideGrid();
    }

    private void CreateGrid()
    {
        for (int x = map.firstCoord.x; x < map.lastCoord.x; x++)
            for (int y = map.firstCoord.y; y < map.lastCoord.y; y++)
                tilemap.SetTile(new Vector3Int(x, y, 0), greenTile);
    }

    public void SetGridColorRed(Vector2Int coord)
    {
        tilemap.SetTile(new Vector3Int(coord.x, coord.y, 0), redTile);
    }

    public void SetGridColorGreen(Vector2Int coord)
    {
        tilemap.SetTile(new Vector3Int(coord.x, coord.y, 0), greenTile);
    }

    public void ShowGrid()
    {
        tilemap.gameObject.SetActive(true);
    }

    public void HideGrid()
    {
        tilemap.gameObject.SetActive(false);
    }
}
