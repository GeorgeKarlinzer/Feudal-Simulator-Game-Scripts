using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class MineMountainGenerator : MonoBehaviour
{
    private List<MineMountain> mountains = new List<MineMountain>();

    public MineMountain mineMountain;

    public Sprite ironSprite;
    public Sprite troleyIron;
    public Sprite stoneSprite;
    public Sprite troleyStone;
    public Sprite goldSprite;
    public Sprite troleyGold;

    public Vector2Int ironNum;
    public Vector2Int stoneNum;
    public Vector2Int goldNum;

    public void Generate()
    {
        GenerateMountain("iron", ironSprite, troleyIron, ironNum);
        GenerateMountain("stone", stoneSprite, troleyStone, stoneNum);
        GenerateMountain("gold", goldSprite, troleyGold, goldNum);
    }

    public void GenerateMountain(string resName, Sprite sprite, Sprite sprite1, Vector2Int num)
    {
        int trueNum = Random.Range(num.x, num.y + 1);
        for (int i = 0; i < trueNum; i++)
        {
            Vector3Int rotation = new Vector3Int(0, 0, Random.Range(0, 4) * 90);
            Vector2Int size = new Vector2Int(2, 3);
            if ((rotation.z / 90) % 2 == 1)
            {
                int a = size.x;
                size.x = size.y;
                size.y = a;
            }

            MapManager map = MapManagerStatic;
            int x = Random.Range(map.firstCoord.x + 3, map.lastCoord.x - 3);
            int y = Random.Range(map.firstCoord.y + 3, map.lastCoord.y - 3);

            while (!map.GetMapFullness(size, new Vector2(x - 0.5f, y - 0.5f) + new Vector2(size.x, size.y) / 2f))
            {
                x = Random.Range(map.firstCoord.x + 3, map.lastCoord.x - 3);
                y = Random.Range(map.firstCoord.y + 3, map.lastCoord.y - 3);
            }

            map.SetMapFullness(size, new Vector2(x - 0.5f, y - 0.5f) + new Vector2(size.x, size.y) / 2f, false);
            var mountain = Instantiate(mineMountain, new Vector2(x - 0.5f, y - 0.5f) + new Vector2(size.x, size.y) / 2f, Quaternion.Euler(rotation));
            mountain.SetMountainType(sprite, sprite1, resName);
            mountains.Add(mountain);
        }
    }

    public void Clear()
    {
        while(mountains.Count > 0)
        {
            MapManagerStatic.SetMapFullness(new Vector2Int(2, 2), mountains[0].transform.position, true);
            Destroy(mountains[0].gameObject);
            mountains.RemoveAt(0);
        }
    }
}
