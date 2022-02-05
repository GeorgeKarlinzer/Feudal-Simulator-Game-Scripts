using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourRelated
{
    private static NeighbourRelated instance;

    public static NeighbourRelated Instance
    {
        get => instance == null ? instance = new NeighbourRelated() : instance;
    }

    private void NeighbourAppears(int delta, INeighbourRelated obj)
    {
        obj.SpriteIndex += delta;
        obj.Renderer.sprite = obj.Sprites[obj.SpriteIndex];
        obj.ThisWasFound?.Invoke(delta);
    }

    public void CheckNeighbors<T>(int s, INeighbourRelated obj) where T : MonoBehaviour, INeighbourRelated
    {
        for (int i = 0; i < 4; i++)
        {
            var origin = obj.Pos2D + new Vector2(Mathf.Cos(i * Mathf.PI / 2), Mathf.Sin(i * Mathf.PI / 2));
            var dir = new Vector2(0, 0);
            var ray = Physics2D.RaycastAll(origin, dir);
            foreach (var r in ray)
                if (r.collider.GetComponent<T>())
                {
                    NeighbourAppears(s * (int)Mathf.Pow(2, 3 - i), obj);
                    var neighbour = r.collider.GetComponent<INeighbourRelated>();
                    NeighbourAppears(s * (int)Mathf.Pow(2, 1 + (i / 2) * 2 - i % 2), neighbour);
                    if (s > 0)
                        obj.NeighbourFound?.Invoke(i);
                }
        }
    }
}
