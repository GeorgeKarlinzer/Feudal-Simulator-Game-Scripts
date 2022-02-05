using Pathfinding;

public class ArcherAIPath : HumanAIPath
{
    private Archer archer;

    protected override void Awake()
    {
        base.Awake();
        archer = GetComponent<Archer>();
    }

    public override void OnTargetReached()
    {
        var temp = deltaTarget;
        deltaTarget = float.MaxValue;
        base.OnTargetReached();
        deltaTarget = temp;
    }

    protected override void OnPathComplete(Path newPath)
    {
        base.OnPathComplete(newPath);
        var enemy = destinationSetter.target.GetComponent<SoldierBase>();

        if (FightHandler.Instance.IsGroupAttack(archer.Group) && archer.CanAttack(enemy))
            OnTargetReached();
    }
}
