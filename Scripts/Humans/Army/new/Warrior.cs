using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : SoldierBase
{
    [SerializeField] private float attackDistance;


    protected override void Awake()
    {
        base.Awake();
        this.AttackPriority = 1;
    }

    private void SetEndReachedDist(float dist) =>
        aIPath.endReachedDistance = dist;

    public override void GoToPos(Vector3 pos)
    {
        SetEndReachedDist(0.01f);
        base.GoToPos(pos);
    }

    public override void GoAttack(SoldierBase enemy, bool isGetWeapon)
    {
        SetEndReachedDist(attackDistance);
        base.GoAttack(enemy, isGetWeapon);
    }
}
