using System.Collections.Generic;
using UnityEngine;

public class BarrackWork : MonoBehaviour
{
    [SerializeField] private Transform[] relaxPlaces;
    [SerializeField] private Transform[] sleepPlaces;
    [SerializeField] private bool[] isBedBusy;
    private List<HumanActivity<Worker>> activities = new List<HumanActivity<Worker>>();
    public List<int> enableActivities = new List<int>();

    void Awake()
    {
        for (var i = 0; i < relaxPlaces.Length; i++)
        {
            var a = i;
            var activity = new HumanActivity<Worker>
            {
                GetPlace = (worker) => relaxPlaces[a],
                CantReachTarget = (worker) => { throw new System.Exception("Караул"); },
                ExecuteAction = Relax,
                FinishActivity = FinishRelax,
                SetNewActivity = SetNewActivity
            };

            activities.Add(activity);
            enableActivities.Add(i);
        }

        var sleepActivity = new HumanActivity<Worker>
        {
            GetPlace = GetSleepPlace,
            CantReachTarget = (worker) => { throw new System.Exception("Караул"); },
            ExecuteAction = Sleep,
            FinishActivity = FinishSleep,
            SetNewActivity = SetNewActivity
        };
        activities.Add(sleepActivity);
        enableActivities.Add(enableActivities.Count);

        var eatActivity = new HumanActivity<Worker>
        {
            GetPlace = CanteenGlobal.GetEatPlace,
            CantReachTarget = CantReachTarget,
            ExecuteAction = Eat,
            FinishActivity = FinishEat,
            SetNewActivity = SetNewActivity
        };
        activities.Add(eatActivity);
        enableActivities.Add(enableActivities.Count);

        isBedBusy = new bool[sleepPlaces.Length];
    }

    public void CantReachTarget(Worker worker)
    {
        worker.StopActivity();
        TeleportToBarrack(worker);
        SetNewActivity(worker);
    }

    public void TeleportToBarrack(Worker worker)
    {
        worker.barrack.GoBack(worker, true);
    }

    public void SetNewActivity(Worker worker)
    {
        var i = enableActivities[Random.Range(0, enableActivities.Count)];

        if (i == activities.Count - 1 && !CanteenGlobal.CanGetEatPlace(worker.transform))
        {
            enableActivities.Remove(i);
            i = enableActivities[Random.Range(0, enableActivities.Count)];
            enableActivities.Add(activities.Count - 1);

        }

        if (i < activities.Count - 2)
        {
            worker.activityIndex = i;
            enableActivities.Remove(i);
        }

        worker.CurrentActivity = activities[i];
    }

    public void Relax(Worker worker)
    {
        worker.SetTrigger("Relax");
        worker.StartExecuteActionTimer(5);
    }

    public void FinishRelax(Worker worker) => enableActivities.Add(worker.activityIndex);

    public Transform GetSleepPlace(Worker worker)
    {
        for (var i = 0; i < sleepPlaces.Length; i++)
            if (!isBedBusy[i])
            {
                isBedBusy[i] = true;
                return sleepPlaces[i];
            }

        throw new System.Exception("Не хватает кроватей!");
    }

    public void Sleep(Worker worker)
    {
        worker.transform.localScale = Vector3.zero;
        worker.GetDestination().GetChild(0).gameObject.SetActive(true);
        worker.StartExecuteActionTimer(5);
    }

    public void FinishSleep(Worker worker)
    {
        worker.transform.localScale = Vector3.one;
        worker.GetDestination().GetChild(0).gameObject.SetActive(false);

        for (var i = 0; i < sleepPlaces.Length; i++)
            if (sleepPlaces[i] == worker.GetDestination())
            {
                isBedBusy[i] = false;
                break;
            }
    }

    public void Eat(Worker worker)
    {
        worker.transform.localScale = Vector3.zero;
        worker.GetDestination().GetChild(0).gameObject.SetActive(true);
        worker.StartExecuteActionTimer(5);
    }

    public void FinishEat(Worker worker)
    {
        var place = worker.GetDestination();

        worker.transform.localScale = Vector3.one;
        place.GetChild(0).gameObject.SetActive(false);
        CanteenGlobal.MakePlaceFree(place);
    }
}
