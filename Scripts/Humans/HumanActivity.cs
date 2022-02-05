using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class HumanActivity<T> where T : IHuman
{
    /// <summary>
    /// Действие возвращающее место работы
    /// </summary>
    public Func<T, Transform> GetPlace;
    /// <summary>
    /// Действие выполняемое, если во время пути, оказалось, что достигнуть цель не получится
    /// </summary>
    public Action<T> CantReachTarget;
    /// <summary>
    /// Основное действие, которое будет выполняться после того, как человек придёт к цели
    /// </summary>
    public Action<T> ExecuteAction;
    /// <summary>
    /// Действие, выполняемое после основного (действие завершающее состояние активности)
    /// </summary>
    public Action<T> FinishActivity;
    /// <summary>
    /// Устанавливает новое действие
    /// </summary>
    public Action<T> SetNewActivity;

    public HumanActivity() { }

    public HumanActivity(HumanActivity<T> activity)
    {
        GetPlace = new Func<T, Transform>(activity.GetPlace);
        if (activity.CantReachTarget != null)
            CantReachTarget = new Action<T>(activity.CantReachTarget);
        ExecuteAction = new Action<T>(activity.ExecuteAction);
        FinishActivity = new Action<T>(activity.FinishActivity);
        SetNewActivity = new Action<T>(activity.SetNewActivity);
    }

    public HumanActivity(Func<T, Transform> GetPlace, Action<T> CantReachTarget, Action<T> ExecuteAction, Action<T> FinishActivity, Action<T> SetNewActivity)
    {
        this.GetPlace = GetPlace;
        if (CantReachTarget != null)
            this.CantReachTarget = CantReachTarget;
        this.ExecuteAction = ExecuteAction;
        this.FinishActivity = FinishActivity;
        this.SetNewActivity = SetNewActivity;
    }
}

public class StringArrEvent : UnityEvent<string[]> { }