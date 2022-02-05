using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lord : SoldierBase
{
    Coroutine actionTimer;
    public new Transform target => base.target;

    private HumanActivity<Lord> normalActivity = new HumanActivity<Lord>()
    {
        GetPlace = l => throw new NotImplementedException(),
        CantReachTarget = l => throw new NotImplementedException(),
        ExecuteAction = l => { l.SetTrigger("Idle"); l.CheckIsInTheBedroom(); },
        FinishActivity = l => { },
        SetNewActivity = l => { }
    };

    private new HumanActivity<Lord> activity;

    private void CheckIsInTheBedroom()
    {
        var ray = Physics2D.RaycastAll(transform.position, Vector2.zero, Mathf.Infinity);
        foreach (var item in ray)
        {
            var l = item.collider.GetComponent<ILordRoom>();
            if (l != null)
                l.SetNewActivity();
        }
    }

    //public override void Send()
        //=> Send(normalActivity);

    public void Send(HumanActivity<Lord> activity)
    {
        StopActivity();
        this.activity = new HumanActivity<Lord>(activity);
        activity.GetPlace(this);
        aIPath.enabled = true;
        aIPath.SearchPath();
        SetTrigger("Run");
    }

    public override void CantReachTarget()
        => activity.CantReachTarget?.Invoke(this);

    public override void ExecuteAction()
        => activity.ExecuteAction(this);

    public void StartExecuteActionTimer(float executingTime)
    => actionTimer = StartCoroutine(ExecuteActionTimer(executingTime));

    public void StopActivity()
    {
        activity?.FinishActivity?.Invoke(this);
        aIPath.enabled = false;
        if (actionTimer != null)
            StopCoroutine(actionTimer);
    }

    IEnumerator ExecuteActionTimer(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        activity.FinishActivity?.Invoke(this);
        activity.SetNewActivity?.Invoke(this);
    }
}