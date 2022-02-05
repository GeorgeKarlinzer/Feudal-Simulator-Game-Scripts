using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

public class HillGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] mountainTile;
    public float noiseScale;
    public float noiseHeight;

    private bool[,] map;
    private MapManager mapManager;
    private Vector2Int mapSize;

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    public void Generate()
    {
        mapManager = MapManagerStatic;
        mapSize = mapManager.Size;
        map = new bool[mapSize.x, mapSize.y];

        float offset = Random.Range(1000, 2000);
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
        for (int y = 0; y < mapSize.y; y++)
            for (int x = 0; x < mapSize.x; x++)
                if (map[x, y])
                    SetTile(x, y);
    }

    private void SetTile(int x, int y)
    {
        int index = GetIndex(x, y);

        map[x, y] = false;
        switch (index)
        {
            case 0:
                return;
            case 1:
                return;
            case 2:
                return;
            case 4:
                ResetTile(x, y - 1);
                return;
            case 5:
                ResetTile(x, y - 1);
                return;
            case 7:
                if (!map[x + 1, y + 1] || !map[x + 1, y - 1])
                {
                    if (map[x + 1, y - 1])
                    {
                        index = 6;
                        break;
                    }
                    ResetTile(x, y - 1);
                    return;
                }
                break;
            case 8:
                ResetTile(x - 1, y);
                return;
            case 10:
                ResetTile(x - 1, y);
                return;
            case 11:
                if (!map[x + 1, y + 1] || !map[x - 1, y + 1])
                {
                    if (map[x - 1, y + 1])
                    {
                        index = 9;
                        break;
                    }
                    ResetTile(x - 1, y);
                    return;
                }
                break;
            case 13:
                if (!map[x - 1, y + 1] || !map[x - 1, y - 1])
                {
                    if (map[x - 1, y - 1])
                    {
                        index = 12;
                        break;
                    }
                    ResetTile(x - 1, y);
                    ResetTile(x, y - 1);
                    return;
                }
                break;
            case 14:
                if (!map[x + 1, y - 1] || !map[x - 1, y - 1])
                {
                    if (map[x - 1, y - 1])
                    {
                        index = 12;
                        break;
                    }
                    if (map[x + 1, y - 1])
                    {
                        index = 6;
                        break;
                    }
                    return;
                }
                break;
            case 15:
                int c = 0;
                if (x + 1 < mapSize.x && y + 1 < mapSize.y && !map[x + 1, y + 1]) c += 1;
                if (x + 1 < mapSize.x && y - 1 >= 0 && !map[x + 1, y - 1]) c += 2;
                if (x - 1 >= 0 && y - 1 >= 0 && !map[x - 1, y - 1]) c += 4;
                if (x - 1 >= 0 && y + 1 < mapSize.y && !map[x - 1, y + 1]) c += 8;

                if (c != 0 && c != 1 && c != 2 && c != 4 && c != 8)
                {
                    if (c == 5)
                    {
                        ResetTile(x - 1, y);
                        ResetTile(x, y - 1);
                        ResetTile(x + 1, y - 1);
                        return;
                    }
                    else
                    if (c == 10)
                    {
                        ResetTile(x - 1, y);
                        ResetTile(x, y - 1);
                        ResetTile(x - 1, y - 1);
                        return;
                    }
                }
                else
                    index = c;
                break;
        }

        map[x, y] = true;
        mapManager.SetMapFullness(Vector2Int.one, new Vector2(x, y), index == 0);
        tilemap.SetTile(new Vector3Int(x, y, 0), mountainTile[index]);
    }

    private void ResetTile(int x, int y)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), null);
        mapManager.SetMapFullness(Vector2Int.one, new Vector2(x, y), true);
        SetTile(x, y);
    }

    private int GetIndex(int x, int y)
    {
        int index = 0;
        if (y + 1 < mapSize.y && map[x, y + 1]) index += 1;
        if (x + 1 < mapSize.x && map[x + 1, y]) index += 2;
        if (y - 1 >= 0 && map[x, y - 1]) index += 4;
        if (x - 1 >= 0 && map[x - 1, y]) index += 8;

        return index;
    }
}
