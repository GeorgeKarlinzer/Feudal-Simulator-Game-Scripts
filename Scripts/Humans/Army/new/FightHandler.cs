using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FightHandler
{
    private static FightHandler instance;
    public static FightHandler Instance => instance == null ? instance = new FightHandler() : instance;

    private Dictionary<GroupBase, GroupBase> fightingGroupsMap = new Dictionary<GroupBase, GroupBase>();
    /// <summary>
    /// Queue of soldiers who attack key-soldier
    /// </summary>
    private Dictionary<SoldierBase, List<SoldierBase>> attackMeMap = new Dictionary<SoldierBase, List<SoldierBase>>();
    private Dictionary<SoldierBase, SoldierBase> iAttackMap = new Dictionary<SoldierBase, SoldierBase>();


    private FightHandler() { }

    public void OnSoldierCreated(SoldierBase soldier)
    {
        attackMeMap.Add(soldier, new List<SoldierBase>());
        iAttackMap.Add(soldier, null);
    }

    public void OnSoldierDead(SoldierBase soldier)
    {
        iAttackMap.Remove(soldier);

        var temp = attackMeMap[soldier];
        attackMeMap.Remove(soldier);

        foreach (var item in attackMeMap)
            item.Value.Remove(soldier);

        foreach (var s in temp)
        {
            s.StopAttack();
            s.GoAttack(GetNewOpponent(s), false);
        }
    }

    public void AddAttacker(SoldierBase attacker, SoldierBase opponent)
    {
        iAttackMap[attacker] = opponent;
        attackMeMap[opponent].Add(attacker);
    }

    public void RemoveAttacker(SoldierBase attacker)
    {
        foreach (var item in attackMeMap)
            item.Value.Remove(attacker);

        iAttackMap[attacker] = null;
    }

    public void Attack(SoldierBase attacker, SoldierBase opponent)
    {
        // Если оппонента никто не бьет, перенаправляем оппонента бить атакующего
        //if (attackMeMap[opponent].Count == 1 && iAttackMap[opponent] != attacker)
        if (iAttackMap[opponent] == null)
        {
            if (!opponent.Group.IsAttack)
            {
                opponent.Group.GoAttack(attacker.Group);
                return;
            }
            opponent.GoAttack(attacker, iAttackMap[opponent] == null);
        }
    }

    public SoldierBase GetNewOpponent(SoldierBase soldier)
    {
        // Абстрактный приоритет Добавляем врагам группы которые меня не атакуют 0.5 к приоритету сортируем всех военных группы врага
        // И берем первого

        var enemySoldiers = fightingGroupsMap[soldier.Group]?.soldiers;

        if (enemySoldiers == null || enemySoldiers.Count == 0)
        {
            // TODO: Add soldiers from another group
            return null;
        }    


        // Using stable sort algorythm, sort soldiers from enemy group by attack me count then by priority (if dont attack soldier +0.5f)
        return enemySoldiers
                .Select(s => (
                            soldier: s, 
                            priority: attackMeMap[soldier].Contains(s) ? s.AttackPriority : s.AttackPriority + 0.5f,
                            attackMeCount: attackMeMap[s].Count))
                .OrderBy(e => e.attackMeCount)
                .OrderBy(e => e.priority)
                .First()
                .soldier;
    }

    public void ContinueOrFinishAttack(GroupBase group)
    {
        fightingGroupsMap[group] = null;
        // If no one attaks soldiers from group, this grop finish attack, else go attack new group
        foreach (var s in group.soldiers)
            if (attackMeMap[s]?.Count > 0)
            {
                group.GoAttack(attackMeMap[s][0].Group, false);
                return;
            }

        group.FinishAttack();
    }

    public void GroupStartsAttack(GroupBase group, GroupBase enemyGroup) =>
        fightingGroupsMap[group] = enemyGroup;

    public void GroupStopsAttack(GroupBase group) =>
        fightingGroupsMap[group] = null;

    public bool IsGroupAttack(GroupBase group) =>
        fightingGroupsMap[group] != null;

    public void RemoveFightingGroup(GroupBase group) =>
        fightingGroupsMap.Remove(group);

    public void AddFightingGroup(GroupBase allyGroup) =>
        fightingGroupsMap.Add(allyGroup, null);

    public GroupBase GetAttackingGroup(GroupBase group) =>
        fightingGroupsMap[group];
}
