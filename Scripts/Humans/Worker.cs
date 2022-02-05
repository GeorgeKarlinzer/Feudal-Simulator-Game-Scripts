using Pathfinding;
using System;
using System.Collections;
using UnityEngine;

public class Worker : MonoBehaviour, IHuman
{
    [SerializeField] private AIDestinationSetter destinationSetter;
    [SerializeField] private AIPath aIPath;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer box;
    [SerializeField] private GameObject wood;
    public Body body;

    private Coroutine actionTimer, resTimer;

    private HumanActivity<Worker> currentActivity;
    public HumanActivity<Worker> CurrentActivity
    {
        get => currentActivity;
        set
        {
            currentActivity = value;
            Send(currentActivity.GetPlace(this));
        }
    }

    public bool DisableWhenReachTarget => true;

    [HideInInspector] public Barrack barrack;

    public GatheringBuilding workPlace;

    public int activityIndex;

    public Transform GetDestination()
        => destinationSetter.target;

    public void SetController(RuntimeAnimatorController controller)
        => animator.runtimeAnimatorController = controller;

    public void SetTrigger(string trigger)
    { 
        if(trigger == "Idle") { }
         animator.SetTrigger(trigger);
    }

    public void StartExecuteActionTimer(float executingTime)
        => actionTimer = StartCoroutine(ExecuteActionTimer(executingTime));

    public void SetBoxSprite(Sprite sprite)
        => box.sprite = sprite;

    public void SetBoxState(bool state)
        => box.gameObject.SetActive(state);

    public void SetWoodState(bool state)
        => wood.SetActive(state);

    public void StartWork(float time)
        => resTimer = StartCoroutine(GettingResourceTimer(time));

    public void CantReachTarget()
        => currentActivity.CantReachTarget?.Invoke(this);

    public void StartGo()
    {
        SetTrigger("Run");
        aIPath.canMove = true;
    }

    public void StopGo()
    {
        SetTrigger("Idle");
        aIPath.canMove = false;
    }

    public void StopActivity()
    {
        currentActivity.FinishActivity?.Invoke(this);
        aIPath.enabled = false;
        if (actionTimer != null)
            StopCoroutine(actionTimer);
    }

    private void Send(Transform target)
    {
        destinationSetter.target = target != null ? target : throw new Exception("Еблан");
        aIPath.enabled = true;
        aIPath.SearchPath();

        if (aIPath.canMove)
            SetTrigger(box.gameObject.activeSelf ? "Box" : "Run");
    }

    public void Wait(Action<Worker> finishActivity)
    {
        var act = new HumanActivity<Worker>(currentActivity);
        act.FinishActivity = new Action<Worker>(finishActivity);
        currentActivity = act;
        SetTrigger("Idle");
        aIPath.enabled = false;
    }

    public void StopWork()
    {
        if (resTimer != null)
            StopCoroutine(resTimer);
    }

    public void CantReachStorage(string[] resNames)
    {
        foreach (var item in resNames)
            if (item == workPlace.ResName)
            {
                CurrentActivity.FinishActivity(this);
                CurrentActivity.CantReachTarget(this);
            }
    }

    IEnumerator ExecuteActionTimer(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        CurrentActivity.FinishActivity?.Invoke(this);
        CurrentActivity.SetNewActivity?.Invoke(this);
    }

    IEnumerator GettingResourceTimer(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        StopActivity();
        workPlace.SendToStorage(this);
    }

    private void OnDestroy()
    {
        if (workPlace != null)
            workPlace.Dismiss(this);
    }

    public void ExecuteAction()
    {
        if (currentActivity.ExecuteAction != null)
            currentActivity.ExecuteAction(this);
        else
            Debug.Log("Не установлено действие по прибытию");
    }
}

[Serializable]
public class Body
{
    [SerializeField] private SpriteRenderer helm;
    [SerializeField] private SpriteRenderer armorL;
    [SerializeField] private SpriteRenderer armorR;

    public void Reset()
    {
        helm.sprite = null;
        armorL.sprite = null;
        armorR.sprite = null;
    }

    public void Set(Employee employee)
    {
        helm.sprite = employee.helm;
        armorL.sprite = employee.armorL;
        armorR.sprite = employee.armorR;
    }
}