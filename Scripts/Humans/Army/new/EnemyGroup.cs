using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyGroup : GroupBase
{
    public override void GoAttack(GroupBase enemyGroup, bool isGetWeapon = true)
    {
        base.GoAttack(enemyGroup, isGetWeapon);
        IsAttack = true;
        enemyGroup.IsAttack = true;
    }

    public override void RemoveSoldier(SoldierBase soldier)
    {
        base.RemoveSoldier(soldier);
        if (soldiers.Count == 0)
            FightHandler.Instance.RemoveFightingGroup(this);
    }
}
