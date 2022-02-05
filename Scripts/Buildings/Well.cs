using System;

public class Well : GatheringBuilding
{
    protected override void SetActivities()
    {
        var animationTrigger = "Well";
        var workingTime = 0f;

        for (int i = 0; i < places.Count; i++)
        {
            var a = i;
            var act = new HumanActivity<Worker>
            {
                GetPlace = (worker) => places[a],
                CantReachTarget = (worker) => worker.StopGo(),
                ExecuteAction = (worker) => StandartExecuteAction(worker, animationTrigger, workingTime),
                FinishActivity = (worker) => { },
                SetNewActivity = (worker) => { }
            };

            activities.Add(act);
        }

        SetGoToStorage();
    }

    public override HumanActivity<Worker> GetNewActivity(out int index)
    {
        index = -1;
        return activities[0];
    }
}
