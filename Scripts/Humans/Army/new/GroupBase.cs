using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public abstract class GroupBase
{
    public readonly int MaxSoldiers;

    private readonly float spacingBetweenSoldiers = 0.3f;

    public Vector3 Center => soldiers[0].Target;

    public List<SoldierBase> soldiers = new List<SoldierBase>();

    public virtual bool IsAttack { get; set; }

    private readonly FightHandler fh = FightHandler.Instance;


    public GroupBase() : this(25) { }

    public GroupBase(int maxSoldiers)
    {
        MaxSoldiers = maxSoldiers;
        fh.AddFightingGroup(this);
    }

    public void FinishAttack()
    {
        IsAttack = false;

        foreach (var s in soldiers)
            s.StopAttack();

        if (soldiers.Count > 0)
            GoTo(soldiers[0].transform.position);

        fh.GroupStopsAttack(this);
    }

    public virtual void GoAttack(GroupBase enemyGroup, bool isGetWeapon = true)
    {
        if (!fh.IsGroupAttack(this))
            fh.GroupStartsAttack(this, enemyGroup);

        for (int i = 0; i < soldiers.Count; i++)
            soldiers[i].GoAttack(fh.GetNewOpponent(soldiers[i]), isGetWeapon);
    }

    public void GoTo(Vector3 position)
    {
        if (MyAstarHandler.IsPathPossible(soldiers[0].transform, position))
        {
            if (fh.IsGroupAttack(this))
                fh.GroupStopsAttack(this);

            foreach (var s in soldiers)
                s.GoToPos(position + s.Offset);
        }
    }

    public virtual void AddSoldier(SoldierBase soldier)
    {
        if (soldiers.Count == MaxSoldiers)
            throw new Exception("Не было проверки значения");

        soldiers.Add(soldier);
        soldier.Group = this;
        soldier.OnDead.AddListener(() => RemoveSoldier(soldier));

        soldier.Offset = CalculateOffset(soldiers.Count - 1).ToVector2() * spacingBetweenSoldiers;
        soldier.GoToPos(Center + soldier.Offset);
    }

    public Tuple<int, int> CalculateOffset(int n)
    {
        if (n == 0) return (0, 0).ToTuple();

        // Some math magic
        var k = 2 * (((int)Mathf.Sqrt(n) + 1) / 2) + 1;
        n = (n - (k - 2) * (k - 2) + 1) % (k / 2 * 8);

        var x = (-n / (k - 1) + 1) % 2 * ((k - 1) / 2);
        var y = (-n / (k - 1) + 2) % 2 * ((k - 1) / 2);

        x = x == 0 ? (-(k / 2) + (n % (k - 1))) * -Math.Sign(y) : x;
        y = y == 0 ? (-(k / 2) + (n % (k - 1))) * Math.Sign(x) : y;

        return (x, y).ToTuple();
    }

    public virtual void RemoveSoldier(SoldierBase soldier)
    {
        soldiers.Remove(soldier);
        FightHandler.Instance.OnSoldierDead(soldier);

        for (int i = 0; i < soldiers.Count; i++)
            soldiers[i].Offset = CalculateOffset(i).ToVector2() * spacingBetweenSoldiers;

        if (soldiers.Count == 0 && fh.IsGroupAttack(this))
        {
            fh.ContinueOrFinishAttack(fh.GetAttackingGroup(this));
            FinishAttack();
        }
    }
}
