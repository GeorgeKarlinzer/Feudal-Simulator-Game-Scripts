using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ExtensionMethods
{
    public static bool Send(this ResManager.Resource res, Transform target, out Worker worker)
    {
        foreach (IStorage s in res.GetStorages())
            if (((Barrack)s).Send(target, out worker))
                return true;

        worker = null;
        return false;
    }

    public static Vector2 ToVector2(this Tuple<int, int> tuple) =>
        new Vector2(tuple.Item1, tuple.Item2);

    public static T MinBy<T>(this IEnumerable<T> collection, Func<T, int> selector)
    {
        var min = collection.First();

        collection
            .ToList()
            .ForEach(x => min = selector(x) < selector(min) ? x : min);

        return min;
    }

    public static T MinBy<T>(this IEnumerable<T> collection, Func<T, float> selector)
    {
        var min = collection.First();

        collection
            .ToList()
            .ForEach(x => min = selector(x) < selector(min) ? x : min);

        return min;
    }

    public static T Pop<T>(this List<T> collection)
    {
        var obj = collection.First();
        collection.RemoveAt(0);
        return obj;
    }

    public static Vector2Int ToV2(this (int, int) tuple) =>
        new Vector2Int(tuple.Item1, tuple.Item2);

    public static Vector2 ToVec(this (float, float) tuple) =>
        new Vector2(tuple.Item1, tuple.Item2);
}
