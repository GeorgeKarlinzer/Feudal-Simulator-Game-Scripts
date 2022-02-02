using UnityEngine;

public class Kitchen : GatheringBuilding
{
    [SerializeField] protected string startWorkTrigger;

    protected override string StartWorkTrigger { get => startWorkTrigger; }

    protected override void SetActivities()
    {
        var trigger = "Kitchen";
        var time = 5;
        base.SetActivities();
        for (var i = 0; i < places.Count; i++)
        {
            var a = i;
            var ac = new HumanActivity<Worker>
            {
                GetPlace = (worker) => { return places[a]; },
                CantReachTarget = (worker) => worker.StopGo(),
                ExecuteAction = (worker) => { StandartExecuteAction(worker, trigger, time); },
                FinishActivity = StandartFinishActivity,
                SetNewActivity = SetNewActivity
            };
            activities.Add(ac);
            enableActivities.Add(i);
        }

        SetGoToStorage();
    }
}
