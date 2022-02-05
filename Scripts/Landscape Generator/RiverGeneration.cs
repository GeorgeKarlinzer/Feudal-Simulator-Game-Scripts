using UnityEngine;
using UnityEngine.Tilemaps;
using static StaticData;

/// <summary>
/// Даже не пытайтесь понять что здесь происходит, это полный разъёб
/// </summary>
public class RiverGeneration : MonoBehaviour
{
    private bool[,] riverTiles;

    public Tilemap tilemap;
    public Tilemap tilemapC;
    public Tile[] riverTile;
    public float minRiverLength;
    public float riverWeight;
    [Range(0f, 1f)]
    public float riverChance;
    private Vector2Int mapSize;
    

    public void Clear()
    {
        tilemap.ClearAllTiles();
        tilemapC.ClearAllTiles();
    }

    public void Generate()
    {
        if (Random.value > riverChance)
            return;

        mapSize = MapManagerStatic.Size;
        riverTiles = new bool[mapSize.x, mapSize.y];

        Vector3Int startPos;
        Vector3Int finishPos;
        while (!GenerateStartAndFinish(out startPos, out finishPos)) { }

        Vector2 vector = new Vector2(finishPos.x - startPos.x, finishPos.y - startPos.y);
        int worldDelta = (int)Mathf.Sqrt(Vector2.SqrMagnitude(vector));
        float mainFi = Mathf.Atan2(vector.y, vector.x);

        var riverPoints = new Vector3Int[0];
        bool flag = false;
        int c = 0;
        while (!flag && ++c < 1000)
        {
            var points = GetPoints(worldDelta, mainFi);
            flag = PointsToWorldSpace(points, startPos, out riverPoints);
        }

        if (riverPoints[riverPoints.Length - 1] != finishPos)
            riverPoints[riverPoints.Length - 1] = finishPos;

        foreach (var v in riverPoints)
            riverTiles[v.x, v.y] = true;

        FillRiverBody(riverPoints);
        SetTiles();
    }

    private void FillRiverBody(Vector3Int[] riverPoints)
    {
        float rW2 = riverWeight * riverWeight;
        for (int i = 1; i < riverPoints.Length; i++)
        {
            Vector2 p0 = new Vector2(riverPoints[i - 1].x, riverPoints[i - 1].y);
            Vector2 p1 = new Vector2(riverPoints[i].x, riverPoints[i].y);

            bool isAInf = p0.x == p1.x;
            bool isA1Inf = p0.y == p1.y;

            float a = (p1.y - p0.y) / (p1.x - p0.x);
            float b = p0.y - p0.x * a;

            float y = Mathf.Sqrt(rW2 / (4 * (a * a + 1)));
            float x = -a * y;
            Vector2 u = !isAInf ? new Vector2(x, y) : new Vector2(riverWeight / 2, 0);
            Vector2 p2 = p0 + u;
            Vector2 p3 = p0 - u;
            Vector2 p4 = p1 + u;
            Vector2 p5 = p1 - u;

            Vector2 p6 = (p0 + p1) / 2;
            float a1 = -1 / a;
            float b1 = p6.y - a1 * p6.x;

            float deltaY1 = !isAInf ? Mathf.Abs(p2.y - p2.x * a - b) : Mathf.Infinity;
            float deltaY2 = !isA1Inf ? Mathf.Abs(p2.y - p2.x * a1 - b1) : Mathf.Infinity;

            int xMin = (int)System.Math.Ceiling(Mathf.Min(p2.x, p3.x, p4.x, p5.x));
            int xMax = (int)System.Math.Floor(Mathf.Max(p2.x, p3.x, p4.x, p5.x));
            int yMin = (int)System.Math.Ceiling(Mathf.Min(p2.y, p3.y, p4.y, p5.y));
            int yMax = (int)System.Math.Floor(Mathf.Max(p2.y, p3.y, p4.y, p5.y));

            for (int newX = xMin; newX <= xMax; newX++)
                for (int newY = yMin; newY <= yMax; newY++)
                {
                    float mainY1 = !isAInf ? a * newX + b : 0;
                    float mainY2 = !isA1Inf ? a1 * newX + b1 : 0;

                    if (newY <= mainY1 + deltaY1 && newY >= mainY1 - deltaY1 && newY <= mainY2 + deltaY2 && newY >= mainY2 - deltaY2)
                        if (newY >= 0 && newY < mapSize.y && newX >= 0 && newX < mapSize.x)
                            riverTiles[newX, newY] = true;
                }
        }
    }

    private bool PointsToWorldSpace(Vector2[] points, Vector3Int startPos, out Vector3Int[] riverPoints)
    {
        int worldDelta = points.Length;
        float delta = Mathf.Sqrt(Vector2.SqrMagnitude(points[points.Length - 1]));

        riverPoints = new Vector3Int[worldDelta];

        for (int i = 0; i < worldDelta; i++)
        {
            float newXPos = points[i].x * worldDelta / delta;
            float newYPos = points[i].y * worldDelta / delta;
            riverPoints[i] = startPos + new Vector3Int((int)newXPos, (int)newYPos, 0);
            if (riverPoints[i].x < 0 || riverPoints[i].x >= mapSize.x || riverPoints[i].y < 0 || riverPoints[i].y >= mapSize.y)
                return false;
        }

        return riverPoints[riverPoints.Length - 1].x % (mapSize.x - 1) == 0 || riverPoints[riverPoints.Length - 1].y % (mapSize.y - 1) == 0;
    }

    private Vector2[] GetPoints(int worldDelta, float mainFi)
    {
        Vector2[] points = new Vector2[worldDelta];

        float x1 = Random.Range(Mathf.PI / 10, 8 * Mathf.PI / 10);
        float delta = Random.Range(Mathf.PI / 10, Mathf.PI / 5);
        float y1 = GetY(x1);

        Vector2 vector = new Vector2(delta, GetY(x1 + delta) - y1);
        float fi = Mathf.Atan2(vector.y, vector.x);

        for (int i = 0; i < worldDelta; i++)
        {
            float xPos = i * delta / worldDelta;
            float yPos = GetY(xPos + x1) - y1;

            RotatePoint(ref xPos, ref yPos, fi - mainFi);
            points[i] = new Vector2(xPos, yPos);
        }

        return points;
    }

    private float GetY(float x)
    {
        return Mathf.Sin(x) * Mathf.Sin(2 * x) * Mathf.Sin(3 * x) * Mathf.Sin(4 * x) * Mathf.Sin(5 * x)
            * Mathf.Sin(6 * x) * Mathf.Sin(7 * x) * Mathf.Sin(8 * x) * Mathf.Sin(9 * x) * Mathf.Sin(10 * x) * 10;
    }

    private void RotatePoint(ref float x, ref float y, float fi)
    {
        float r = Mathf.Sqrt(x * x + y * y);
        float fi0 = Mathf.Atan2(y, x);
        x = r * Mathf.Cos(fi0 - fi);
        y = r * Mathf.Sin(fi0 - fi);
    }

    private bool GenerateStartAndFinish(out Vector3Int startPos, out Vector3Int finishPos)
    {
        startPos = Vector3Int.zero;
        finishPos = Vector3Int.zero;
        int mx = mapSize.x;
        int my = mapSize.y;

        int max = 2 * (mx + my - 2);
        int pos = Random.Range(1, max - 2);
        if (pos == 49 || pos == 50) pos += 2;

        if (pos < max / 2 + 2)
        {
            startPos.x = pos % mx;
            startPos.y = pos / mx * (my - 1);
        }
        else
        {
            startPos.y = (pos - mx * 2) % (my - 2) + 1;
            startPos.x = pos / (max / 2 + mx) * (mx - 1);
        }

        int x = startPos.x;
        int y = startPos.y;
        finishPos.x = x % (mx - 1) == 0 ? Random.Range(x == 0 ? mx / 2 : 0, x == 0 ? mx : mx / 2) : x / (mx / 2) * (mx - 1);
        finishPos.y = y % (my - 1) == 0 ? Random.Range(y == 0 ? my / 2 : 0, y == 0 ? my : my / 2) : y / (my / 2) * (my - 1);

        return Vector3.SqrMagnitude(finishPos - startPos) > minRiverLength * minRiverLength;
    }

    private void SetTiles()
    {
        MapManager mapManager = MapManagerStatic;
        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                if (riverTiles[x, y])
                {
                    mapManager.SetMapFullness(Vector2Int.one, new Vector2(x, y), false);
                    int index = 0;
                    if (y + 1 >= mapSize.y || riverTiles[x, y + 1]) index += 1;
                    if (x + 1 >= mapSize.x || riverTiles[x + 1, y]) index += 2;
                    if (y - 1 < 0 || riverTiles[x, y - 1]) index += 4;
                    if (x - 1 < 0 || riverTiles[x - 1, y]) index += 8;

                    int indexC = 16;
                    if (x + 1 < mapSize.x && y + 1 < mapSize.y && riverTiles[x + 1, y] && riverTiles[x, y + 1] && !riverTiles[x + 1, y + 1]) indexC += 1;
                    if (x + 1 < mapSize.x && y - 1 >= 0 && riverTiles[x + 1, y] && riverTiles[x, y - 1] && !riverTiles[x + 1, y - 1]) indexC += 2;
                    if (x - 1 >= 0 && y - 1 >= 0 && riverTiles[x - 1, y] && riverTiles[x, y - 1] && !riverTiles[x - 1, y - 1]) indexC += 4;
                    if (x - 1 >= 0 && y + 1 < mapSize.y && riverTiles[x - 1, y] && riverTiles[x, y + 1] && !riverTiles[x - 1, y + 1]) indexC += 8;

                    tilemap.SetTile(new Vector3Int(x, y, 0), riverTile[index]);
                    tilemapC.SetTile(new Vector3Int(x, y, 0), riverTile[indexC]);
                }
    }
}
