using System.Collections.Generic;
using UnityEngine;

public class Arsenal : Building
{
    [SerializeField] private Transform place;
    [SerializeField] private SoldierBase[] soldierPrefab;

    private GroupBase group;

    public void Awake()
    {
        group = new AllyGroup();
    }

    public void Employ(int i)
    {
        if (group.soldiers.Count == group.MaxSoldiers)
            return;

        var s = Instantiate(soldierPrefab[i], place.position, Quaternion.identity);
        group.AddSoldier(s);
        s.gameObject.name = $"Ally{group.soldiers.Count}";
    }

    public void Dismiss()
    {
        if (group.soldiers.Count > 0)
            Destroy(group.soldiers[0]);
    }
}
