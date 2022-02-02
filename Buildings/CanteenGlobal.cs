using System.Collections.Generic;
using UnityEngine;

public static class CanteenGlobal
{
    private static List<CanteenPlaces> canteenPlaces = new List<CanteenPlaces>();

    static public bool CanGetEatPlace(Transform transform)
    {
        foreach (var c in canteenPlaces)
            if (MyAstarHandler.IsPathPossible(transform, c.canteen))
                return true;

        return false;
    }

    static public Transform GetEatPlace(Worker human)
    {
        foreach (var c in canteenPlaces)
            if (MyAstarHandler.IsPathPossible(human.transform, c.canteen))
                for (int i = 0; i < c.isBusy.Length; i++)
                    if (!c.isBusy[i])
                    {
                        c.isBusy[i] = true;
                        return c.places[i];
                    }

        return null;
    }

    static public void AddCanteenPlaces(Transform[] places, Transform canteen) => canteenPlaces.Add(new CanteenPlaces(places, canteen));

    static public void RemoveCanteenPlaces(Transform canteen)
    {
        foreach (var c in canteenPlaces)
            if (c.canteen == canteen)
            {
                canteenPlaces.Remove(c);
                return;
            }
    }

    static public void MakePlaceFree(Transform place)
    {
        foreach (var c in canteenPlaces)
            for (int i = 0; i < c.isBusy.Length; i++)
                if (c.isBusy[i] && c.places[i] == place)
                {
                    c.isBusy[i] = false;
                    return;
                }
    }
}

public class CanteenPlaces
{
    public CanteenPlaces(Transform[] places, Transform canteen)
    {
        this.canteen = canteen;
        this.places = places;
        isBusy = new bool[places.Length];
    }

    public Transform canteen;
    public Transform[] places;
    public bool[] isBusy;
}
