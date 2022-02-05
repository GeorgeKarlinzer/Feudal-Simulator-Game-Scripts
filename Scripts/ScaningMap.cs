using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class ScaningMap : MonoBehaviour
{
    private void Start()
    {
        var startC = MapManagerStatic.firstCoord;
        var endC = MapManagerStatic.lastCoord;

        for (int x = startC.x; x < endC.x; x++)
        {
            for (int y = startC.y; y < endC.y; y++)
            {
                var pos = (x, y).ToV2();
                var size = Vector2.one;
                var angle = 0;
                var dir = Vector2.zero;
                var dist = Mathf.Infinity;
                var layer = LayerMask.GetMask("Obstacle");

                var hit = Physics2D.BoxCast(pos, size, angle, dir, dist, layer);
                if (hit.collider != null)
                    MapManagerStatic.SetMapFullness(Vector2Int.one, pos, false);
            }
        }
    }
}
