using UnityEngine;
using static StaticData;


public class Sawmill : GatheringBuilding
{
    [SerializeField] private Animator roomAnimator;
    [SerializeField] private Transform targetPrefab;
    /* Activities
     * 0) Рубить в лесопилке (Anywhen -)
     * 1) Пилить в лесопилке (Anywhen -)
     * 2) Рубить дерево в лесу (Anywhen +)
     * 3) Принести дерево на пилу (After 2)
     * 4) Принести дерево на кучу (After 2)
     * 5) Нажать на рычаг (After 3 or 6)
     * 6) Перенести дерево с кучи на пилу (Anywhen -)
     */
    protected override void SetActivities()
    {
        var a0 = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[0],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => StandartExecuteAction(worker, "Axe", 5),
            FinishActivity = StandartFinishActivity,
            SetNewActivity = SetNewActivity
        };
        activities.Add(a0);

        var a1 = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[1],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => StandartExecuteAction(worker, "Saw", 5),
            FinishActivity = StandartFinishActivity,
            SetNewActivity = SetNewActivity
        };
        activities.Add(a1);

        var a2 = new HumanActivity<Worker>
        {
            GetPlace = GetTreeTransform,
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => StandartExecuteAction(worker, "Tree", 1),
            FinishActivity = (worker) => ForestStatic.AddTree(worker.GetDestination().position),
            SetNewActivity = ActionAfterTree
        };
        activities.Add(a2);

        var a3 = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[2],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => { roomAnimator.SetTrigger("Saw"); worker.StartExecuteActionTimer(0); },
            FinishActivity = (worker) => worker.SetWoodState(false),
            SetNewActivity = (worker) => worker.CurrentActivity = activities[5]
        };
        activities.Add(a3);

        var a4 = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[3],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => worker.StartExecuteActionTimer(0),
            FinishActivity = (worker) => worker.SetWoodState(false),
            SetNewActivity = SetNewActivity
        };
        activities.Add(a4);

        var a5 = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[4],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => worker.StartExecuteActionTimer(0),
            FinishActivity = (worker) => { },
            SetNewActivity = SetNewActivity
        };
        activities.Add(a5);

        var a6 = new HumanActivity<Worker>
        {
            GetPlace = (worker) => places[3],
            CantReachTarget = (worker) => worker.StopGo(),
            ExecuteAction = (worker) => { worker.SetWoodState(true); worker.CurrentActivity = activities[3]; },
            FinishActivity = StandartFinishActivity,
            SetNewActivity = (worker) => { }

        };
        activities.Add(a6);

        enableActivities.AddRange(new[] { 0, 1, 2, 3 });

        SetGoToStorage();
    }

    public override HumanActivity<Worker> GetNewActivity(out int index)
    {
        index = enableActivities[Random.Range(0, enableActivities.Count)];
        var i = index;
        if (index == 2) index = -1;

        // Отнести на пилу можно только с кучи (имеется ввиду из случайного положения)
        if (index == 3) i = 6;

        return activities[i];
    }

    public void AfterSawAnimation()
        => enableActivities.Add(3);

    public Transform GetTreeTransform(Worker worker)
    {
        var treePos = ForestStatic.GetNearestNeighbours(Destination);
        return Instantiate(targetPrefab, treePos, Quaternion.identity);
    }

    public void ActionAfterTree(Worker worker)
    {
        ForestStatic.CutDownTree(worker.GetDestination().position);
        Destroy(worker.GetDestination().gameObject);

        worker.SetWoodState(true);

        if (enableActivities.Contains(3))
        {
            enableActivities.Remove(3);
            worker.activityIndex = 3;
            worker.CurrentActivity = activities[3];
        }
        else
            worker.CurrentActivity = activities[4];
    }
}
