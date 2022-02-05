using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private RedactingGrid redactingGrid;

    public Vector2Int firstCoord;
    public Vector2Int lastCoord;

    public Vector2Int Size { get => lastCoord - firstCoord; }

    private bool[,] mapFullness;

    private void Awake()
    {
        int X = lastCoord.x - firstCoord.x;
        int Y = lastCoord.y - firstCoord.y;
        mapFullness = new bool[X, Y];

        for (int x = 0; x < lastCoord.x - firstCoord.x; x++)
            for (int y = 0; y < lastCoord.y - firstCoord.y; y++)
                mapFullness[x, y] = true;
    }

    public void ClearMap()
    {
        SetMapFullness(Size, (lastCoord + firstCoord) / 2, true);
    }

    public void SetMapFullness(Vector2Int size, Vector2 pos, bool value)
    {
        Vector2Int firstCoord =
            new Vector2Int((int)(pos.x - 0.5f * (size.x - 1)), (int)(pos.y - 0.5f * (size.y - 1)));

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int map = PosToMap(firstCoord + new Vector2Int(x, y));
                mapFullness[map.x, map.y] = value;

                if (value)
                    redactingGrid.SetGridColorGreen(map);
                else
                    redactingGrid.SetGridColorRed(map);
            }
    }

    public bool GetMapFullness(Vector2Int size, Vector2 pos)
    {
        if (pos.x - size.x / 2f < firstCoord.x - 0.5f || pos.x + size.x / 2f > lastCoord.x ||
            pos.y - size.y / 2f < firstCoord.y - 0.5f || pos.y + size.y / 2f > lastCoord.y)
            return false;

        Vector2Int firstCoord1 =
            new Vector2Int((int)(pos.x - 0.5f * (size.x - 1)), (int)(pos.y - 0.5f * (size.y - 1)));

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int map = PosToMap(firstCoord1 + new Vector2Int(x, y));

                if (!mapFullness[map.x, map.y])
                    return false;
            }

        return true;
    }

    public bool GetMapFullnessInGrid(Vector2Int pos) => mapFullness[PosToMap(pos).x, PosToMap(pos).y];

    private Vector2Int PosToMap(Vector2Int pos)
        => new Vector2Int(pos.x - firstCoord.x, pos.y - firstCoord.y);
}
