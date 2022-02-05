using UnityEngine;

public class Canteen : Building
{
    [SerializeField] private Transform destination;
    [SerializeField] private Transform[] eatPlaces;

    protected override void OnEnable()
    {
        base.OnEnable();
        CanteenGlobal.AddCanteenPlaces(eatPlaces, destination);        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CanteenGlobal.RemoveCanteenPlaces(destination);
    }
}
