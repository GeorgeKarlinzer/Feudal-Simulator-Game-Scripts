using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyGroup : GroupBase
{
    public bool CanInteract { get; set; } = true;

    private List<Ally> allies = new List<Ally>();

    
    public AllyGroup() : base() { }

    public AllyGroup(int maxSoldiers) : base(maxSoldiers) { }

    public override void AddSoldier(SoldierBase soldier)
    {
        base.AddSoldier(soldier);
        allies.Add(soldier.GetComponent<Ally>());
        soldier.OnDead.AddListener(() => allies.Remove(soldier.GetComponent<Ally>()));
    }

    public void SetGroupElipceState(bool state) =>
        allies.ForEach(a => a.SetElipceState(state));
}
