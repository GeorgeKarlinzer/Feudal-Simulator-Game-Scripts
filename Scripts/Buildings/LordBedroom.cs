using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordBedroom : Building, ILordRoom
{
    [SerializeField] private Transform destination;
    [SerializeField] private Lord lordPrefab;
    static public Lord lord { get; private set; }

    GroupBase group = new AllyGroup(1);

    [SerializeField] private List<Transform> places;
    private List<HumanActivity<Lord>> activities;

    private void Awake()
    {
        activities = new List<HumanActivity<Lord>>();
        var activity = new HumanActivity<Lord>
        {
            GetPlace = l => { l.target.position = places[0].position; return l.target; },
            CantReachTarget = null,
            ExecuteAction = l => { l.SetTrigger("Idle"); l.StartExecuteActionTimer(3); },
            FinishActivity = l => { },
            SetNewActivity = l => SetNewActivity()
        };

        activities.Add(activity);

        activity = new HumanActivity<Lord>
        {
            GetPlace = l => { l.target.position = places[1].position; return l.target; },
            CantReachTarget = null,
            ExecuteAction = l => { l.SetTrigger("Idle"); l.StartExecuteActionTimer(3); },
            FinishActivity = l => { },
            SetNewActivity = l => SetNewActivity()
        };

        activities.Add(activity);

        activity = new HumanActivity<Lord>
        {
            GetPlace = l => { l.target.position = places[2].position; return l.target; },
            CantReachTarget = null,
            ExecuteAction = l => { l.SetTrigger("Idle"); l.StartExecuteActionTimer(3); },
            FinishActivity = l => { },
            SetNewActivity = l => SetNewActivity()
        };

        activities.Add(activity);

        activity = new HumanActivity<Lord>
        {
            GetPlace = l => { l.target.position = places[3].position; return l.target; },
            CantReachTarget = null,
            ExecuteAction = l => { l.SetTrigger("SwordLooking"); l.StartExecuteActionTimer(3); },
            FinishActivity = l => { },
            SetNewActivity = l => SetNewActivity()
        };

        activities.Add(activity);

        activity = new HumanActivity<Lord>
        {
            GetPlace = l => { l.target.position = places[4].position; return l.target; },
            CantReachTarget = null,
            ExecuteAction = l => { Eat(); l.StartExecuteActionTimer(3); },
            FinishActivity = l => { FinishEat(); },
            SetNewActivity = l => SetNewActivity()
        };

        activities.Add(activity);
    }

    public void SetNewActivity()
    {
        lord.Send(activities[Random.Range(0, activities.Count)]);
    }

    private void SpawnLord()
    {
        lord = Instantiate(lordPrefab, destination.position, Quaternion.identity);
        lord.Group = group;
        lord.target.parent = null;
        group.AddSoldier(lord);
        SetNewActivity();
    }

    public override void Build()
    {
        base.Build();
        SpawnLord();
    }

    Vector3? beforeEatPos = null;
    Quaternion? beforeEatRot = null;
    public void Eat()
    {
        lord.SetTrigger("Eat");
        beforeEatPos = lord.transform.position;
        beforeEatRot = lord.transform.rotation;
        lord.transform.position = places[4].GetChild(0).position;
        lord.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public void FinishEat()
    {
        if (beforeEatRot != null)
        {
            lord.transform.position = (Vector3)beforeEatPos;
            lord.transform.rotation = (Quaternion)beforeEatRot;
            beforeEatPos = null;
            beforeEatRot = null;
        }
    }
}
