using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordCabinet : Building, ILordRoom
{
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
            ExecuteAction = l => { Seat(); l.StartExecuteActionTimer(3); },
            FinishActivity = l => { FinishSeat(); },
            SetNewActivity = l => SetNewActivity()
        };

        activities.Add(activity);
    }

    public void SetNewActivity()
    {
        LordBedroom.lord.Send(activities[Random.Range(0, activities.Count)]);
    }

    Vector3? beforeEatPos = null;
    Quaternion? beforeEatRot = null;
    public void Seat()
    {
        LordBedroom.lord.SetTrigger("Idle");
        beforeEatPos = LordBedroom.lord.transform.position;
        beforeEatRot = LordBedroom.lord.transform.rotation;
        LordBedroom.lord.transform.position = places[2].GetChild(0).position;
        LordBedroom.lord.transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public void FinishSeat()
    {
        if (beforeEatRot != null)
        {
            LordBedroom.lord.transform.position = (Vector3)beforeEatPos;
            LordBedroom.lord.transform.rotation = (Quaternion)beforeEatRot;
            beforeEatPos = null;
            beforeEatRot = null;
        }
    }
}
