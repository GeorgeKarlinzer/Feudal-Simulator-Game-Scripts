using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using static StaticData;

public abstract class SoldierBase : MonoBehaviour, IHuman
{
    [SerializeField] protected AIDestinationSetter destinationSetter;
    [SerializeField] protected HumanAIPath aIPath;
    [SerializeField] protected Animator animator;
    [SerializeField] protected BodyCollider bodyCollider;

    [SerializeField] protected string getWeaponTrigger;
    [SerializeField] protected string attackTrigger;
    [SerializeField] protected Coroutine hitCoroutine;

    [SerializeField] protected Transform target;
    public Vector3 Target => destinationSetter.target.position;

    public GroupBase Group { get; set; }
    public Vector3 Offset { get; set; }

    public const int MaxHp = 4;
    private int hp = MaxHp;
    public int HP
    {
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)
                Destroy(gameObject);
        }
    }

    public int AttackPriority { get; protected set; }

    public int Damage { get; set; } = 1;

    protected HumanActivity<SoldierBase> activity = new HumanActivity<SoldierBase>()
    {
        CantReachTarget = w => w.StopGo(),
        // TODO: Сделать поворот в конце пути у всех одинаковым
    };

    [HideInInspector] public UnityEvent OnDead = new UnityEvent();


    ~SoldierBase()
    {
        Debug.Log("Destoyed");
    }

    protected virtual void Awake()
    {
        target.position = transform.position;
        target.parent = null;
        FightHandler.Instance.OnSoldierCreated(this);
    }

    private void OnDestroy()
    {
        OnDead.Invoke();
        OnDead.RemoveAllListeners();
        Destroy(target.gameObject);
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);
    }

    public virtual void GoToPos(Vector3 pos)
    {
        activity.FinishActivity?.Invoke(this);

        target.position = pos;
        destinationSetter.target = target;
        activity.ExecuteAction = s => s.SetTrigger("Idle");
        SetTrigger("Run");
        StartGo();
    }

    public void StartGo()
    {
        aIPath.enabled = true;
        aIPath.canMove = true;
    }

    public void StopGo()
    {
        SetTrigger("Idle");
        aIPath.canMove = false;
    }

    public void SetTrigger(string trigger) =>
        animator.SetTrigger(trigger);

    public virtual void GoAttack(SoldierBase enemy, bool isGetWeapon = true)
    {
        if (enemy == null)
            return;

        activity.FinishActivity?.Invoke(this);

        destinationSetter.target = enemy.transform;
        FightHandler.Instance.AddAttacker(this, enemy);

        activity.ExecuteAction = s => s.Attack(enemy);

        SetTrigger(isGetWeapon ? getWeaponTrigger : "WeaponRun");

        StartGo();
    }

    public virtual void Attack(SoldierBase enemy)
    {
        SetTrigger(attackTrigger);
        hitCoroutine = StartCoroutine(HitEnemyRoutine(enemy));
        bodyCollider.Enabled = true;
        FightHandler.Instance.Attack(this, enemy);
    }

    public void StopAttack()
    {
        StopCoroutine(hitCoroutine);
        bodyCollider.Enabled = false;
        FightHandler.Instance.RemoveAttacker(this);
    }

    public virtual void ExecuteAction()
    {
        activity.ExecuteAction.Invoke(this);
    }

    public virtual void CantReachTarget()
    {
        activity.CantReachTarget.Invoke(this);
    }

    protected virtual IEnumerator HitEnemyRoutine(SoldierBase enemy)
    {
        var deltaTime = 1f;
        while (true)
        {
            yield return new WaitForSeconds(deltaTime);
            enemy.HP -= Damage;
        }
    }
}
