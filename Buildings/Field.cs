using System;

public class Field : GatheringBuilding
{
    protected override void SetActivities()
    {
        base.SetActivities();

        var animationTrigger = "Farm";
        var workingTime = 5f;

        for (var i = 0; i < places.Count; i++)
        {
            var a = i;
            var ac = new HumanActivity<Worker>
            {
                GetPlace = (worker) => places[a],
                CantReachTarget = (worker) => worker.StopGo(),
                ExecuteAction = (worker) => StandartExecuteAction(worker, animationTrigger, workingTime),
                FinishActivity = StandartFinishActivity,
                SetNewActivity = SetNewActivity
            };

            activities.Add(ac);
            enableActivities.Add(i);
        }

        SetGoToStorage();
    }
}
