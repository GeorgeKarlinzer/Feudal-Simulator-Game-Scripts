using KdTree;
using KdTree.Math;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class DoorPlace : MonoBehaviour
{
    private static readonly List<SpriteRenderer> doorPlaces = new List<SpriteRenderer>(0);
    private static KdTree<float, float> tree = new KdTree<float, float>(2, new FloatMath());

    public static int TreeCount { get => tree.Count; }

    public static Vector3 GetNearestNeighbour(Vector3 pos)
    {
        var a = tree.GetNearestNeighbours(new[] { pos.x, pos.y }, 1)[0].Point;
        return new Vector3(a[0], a[1], 0);
    }

    public static void Add(SpriteRenderer item)
    {
        item.color -= new Color(0, 0, 0, 1);
        doorPlaces.Add(item);
        var pos = item.transform.position;
        tree.Add(new[] { pos.x, pos.y }, 0);
    }

    public static void Remove(SpriteRenderer item)
    {
        item.color -= new Color(0, 0, 0, 1);
        doorPlaces.Remove(item);
        var pos = item.transform.position;
        tree.RemoveAt(new[] { pos.x, pos.y });
    }

    public static bool Contains(SpriteRenderer item)
    {
        return doorPlaces.Contains(item);
    }

    public static void Show()
    {
        foreach (var place in doorPlaces)
        {
            Color c = place.color;
            place.color = new Color(c.r, c.g, c.b, 1);
        }
    }

    public static void Hide()
    {
        foreach (var place in doorPlaces)
        {
            Color c = place.color;
            place.color = new Color(c.r, c.g, c.b, -1);
        }
    }

    private void OnEnable()
    {
        Vector3 dir = new Vector3(0, 0, 1);
        var hits = Physics2D.RaycastAll(transform.position, dir, Mathf.Infinity);
        foreach (var hit in hits)
            if (hit.collider.GetComponent<DoorPlace>() && hit.transform != transform)
            {
                doorPlaces.Add(GetComponent<SpriteRenderer>());
                doorPlaces.Add(hit.transform.GetComponent<SpriteRenderer>());

                var pos = transform.position;
                tree.Add(new[] { pos.x, pos.y }, 0);
                break;
            }
    }

    private void OnDisable()
    {
        Vector3 dir = new Vector3(0, 0, 1);
        var hits = Physics2D.RaycastAll(transform.position, dir, Mathf.Infinity);
        foreach (var hit in hits)
        {
            if (hit.collider.GetComponent<DoorPlace>() && hit.transform != transform)
            {
                doorPlaces.Remove(GetComponent<SpriteRenderer>());
                doorPlaces.Remove(hit.transform.GetComponent<SpriteRenderer>());

                var pos = transform.position;
                tree.RemoveAt(new[] { pos.x, pos.y });
            }

            Door door = hit.transform.GetComponent<Door>();
            if (door && !door.isStatic)
            {
                door.Destroy();
                BuildMenuStatic.FreeDoors++;
            }
        }
    }
}
