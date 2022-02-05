using System.Collections;
using UnityEngine;

public class Archer : SoldierBase
{
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float attackDistance;
    private SoldierBase opponent;


    protected override void Awake()
    {
        base.Awake();
        AttackPriority = 2;
    }

    public bool CanAttack(SoldierBase enemy)
    {
        var deltaPos = (Vector2)enemy.transform.position - (Vector2)transform.position;
        var deltaDist = deltaPos.magnitude;

        if (deltaDist > attackDistance)
            return false;

        var ray = Physics2D.RaycastAll(transform.position, deltaPos, deltaDist, obstacleMask);

        return ray.Length == 0;
    }

    public override void Attack(SoldierBase enemy)
    {
        hitCoroutine = StartCoroutine(HitEnemyRoutine(enemy));
        bodyCollider.Enabled = true;
        FightHandler.Instance.Attack(this, enemy);
    }

    // Вызывается из анимации
    public void TakeShot()
    {
        if (opponent != null && opponent.HP > 0)
            opponent.HP -= Damage;
        // TODO: Flying arrow
    }

    protected override IEnumerator HitEnemyRoutine(SoldierBase enemy)
    {
        float deltaTime = 2f;

        while (enemy != null && enemy.HP > 0)
        {
            if (!CanAttack(enemy))
            {
                GoAttack(enemy, false);
                break;
            }
            opponent = enemy;
            SetTrigger(attackTrigger);

            yield return new WaitForSecondsRealtime(deltaTime);
        }
    }
}
