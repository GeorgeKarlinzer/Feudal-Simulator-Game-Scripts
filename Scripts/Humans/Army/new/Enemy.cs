using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SoldierBase soldier;
    public EnemyGroup Group => (EnemyGroup)soldier.Group;


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(!Group.IsAttack && collision.TryGetComponent(out Ally s))
            soldier.Group.GoAttack(s.Group);
    }
}