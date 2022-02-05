using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class GatheringBuilding : Building, IStorage, IEmployable
{
    [SerializeField] protected Transform destination;
    [SerializeField] protected Animator animator;
    [SerializeField] protected RuntimeAnimatorController workerController;
    protected virtual string StartWorkTrigger { get; }

    public int EmployedHumans { get => workers.Count; }
    private List<Worker> workers = new List<Worker>(0);

    protected Storage storage;
    [SerializeField] protected int maxAmount;

    [SerializeField] protected string resName;
    public string ResName { get => resName; }

    public Employee employee;

    public int MaxWorkers { get => maxWorkers; }
    [SerializeField] private int maxWorkers;

    public float secondsPerResource;

    [SerializeField] protected List<Transform> places = new List<Transform>();
    public List<HumanActivity<Worker>> activities = new List<HumanActivity<Worker>>();
    public List<int> enableActivities = new List<int>();
    protected HumanActivity<Worker> GoToStorage = new HumanActivity<Worker>();

    private StringArrEvent FindNewStorage = new StringArrEvent();

    public override void Build()
    {
        base.Build();
        storage = new Storage(gameObject, maxAmount, resName);
        ResManager.res[resName].AddStorage(this);
        SetActivities();
    }


    private void OnDestroy()
    {
        ResManager.res[resName].RemoveStorage(this);
    }

    /// <summary>
    /// Устанавливает действия выполняемые в этой комнате
    /// </summary>
    protected virtual void SetActivities() { }

    /// <summary>
    /// Устанавливает действие, когда нужно отнести ресурсы в хранилище
    /// !Место хранилища устанавливается, непосредственно перед выполнением этого действия
    /// </summary>
    protected virtual void SetGoToStorage()
    {
        GoToStorage = new HumanActivity<Worker>
        {
            GetPlace = null,
            CantReachTarget = SendToStorage,
            ExecuteAction = null,
            FinishActivity = null,
            SetNewActivity = ActivityAfterGoToStorage
        };
    }

    public virtual void SetNewActivity(Worker worker)
    {
        worker.CurrentActivity = GetNewActivity(out int index);
        if (index >= 0)
            enableActivities.Remove(index);
        worker.activityIndex = index;
    }

    public virtual HumanActivity<Worker> GetNewActivity(out int index)
    {
        index = enableActivities[UnityEngine.Random.Range(0, enableActivities.Count)];
        return activities[index];
    }

    public virtual void SendToStorage(Worker worker)
    {
        worker.SetBoxState(true);

        var target = ResManager.res[ResName].GetStorage(worker, this);
        if (target != null)
        {
            target.AddListenerFindNewStorage(worker.CantReachStorage);

            GoToStorage.GetPlace = (worker1) => target.Destination;
            GoToStorage.ExecuteAction = (worker1) => { GoToStorage.FinishActivity?.Invoke(worker1); StandartTakeResAction(worker1, target); GoToStorage.SetNewActivity.Invoke(worker1); };
            GoToStorage.FinishActivity = (worker1) => { worker1.SetBoxState(false); target.RemoveListenerFindNewStorage(worker1.CantReachStorage); };
            worker.CurrentActivity = GoToStorage;
        }
        else
        {
            worker.Wait((worker1) => worker1.SetBoxState(false));
            HumanWorkGlobal.AddWaiter(ResName, 1, worker);
        }
    }

    public virtual void ActivityAfterGoToStorage(Worker worker)
    {
        var activity = new HumanActivity<Worker>(GetNewActivity(out int index));
        enableActivities.Remove(index);
        worker.activityIndex = index;
        void ExecuteAction(Worker worker1) { activity.ExecuteAction(worker1); worker1.StartWork(secondsPerResource); }
        worker.CurrentActivity = new HumanActivity<Worker>(activity.GetPlace, activity.CantReachTarget, ExecuteAction, activity.FinishActivity, activity.SetNewActivity);
    }

    public void StandartTakeResAction(Worker worker, IStorage storage, int amount = 1)
        => storage.AddAmount(ResName, amount);

    public void StandartCantReachTarger(Worker worker)
        => worker.StopActivity();

    /// <summary>
    /// Стандартная работа рабочего
    /// </summary>
    /// <param name="worker"></param>
    public void StandartExecuteAction(Worker human, string trigger, float time)
    {
        human.SetTrigger(trigger);
        human.StartExecuteActionTimer(time);
    }

    public void StandartFinishActivity(Worker worker)
    {
        enableActivities.Add(worker.activityIndex);
    }

    /// <summary>
    /// Нанимает рабочего
    /// </summary>
    public virtual void Employ()
    {
        var humanRes = ResManager.res[ResManager.Human];

        if (humanRes.Send(Destination, out Worker worker))
        {
            if (animator && workers.Count == 0)
                animator.SetTrigger(StartWorkTrigger);

            workers.Add(worker);
            worker.body.Set(employee);
            worker.StopActivity();
            worker.workPlace = this;
            worker.SetBoxSprite(ResManager.res[resName].BoxSprite);

            var activity = new HumanActivity<Worker>(GetNewActivity(out int index));
            if (index >= 0)
                enableActivities.Remove(index);
            worker.activityIndex = index;

            void ExecuteAction(Worker worker1) { activity.ExecuteAction(worker1); worker1.StartWork(secondsPerResource); }

            worker.SetController(workerController);
            worker.CurrentActivity = new HumanActivity<Worker>(activity.GetPlace, activity.CantReachTarget, ExecuteAction, activity.FinishActivity, activity.SetNewActivity);
        }
        else
            Debug.Log("Не удалось отправить рабочего!");
    }

    public virtual void Dismiss(Worker worker = null, bool instantly = false)
    {
        if (worker == null)
            if (workers.Count > 0)
                worker = workers[0];
            else
                return;

        worker.body.Reset();
        worker.StopActivity();
        worker.StopWork();
        worker.workPlace = null;
        workers.Remove(worker);
        if (animator && workers.Count == 0)
            animator.SetTrigger("Idle");
        worker.barrack.GoBack(worker, instantly);
    }

    public override void Destroy()
    {
        while (workers.Count > 0)
            Dismiss(workers[0], true);

        base.Destroy();
    }

    // Реализация интерфейса IStorage
    public virtual void AddAmount(string name, int value)
    {
        storage.Amount += value;
        if (storage.Amount == storage.MaxAmount)
            FindNewStorage.Invoke(new string[] { name });

        UpdateUI.Invoke();
        if (value < 0) HumanWorkGlobal.CheckWaiters(name, this);
    }

    public int GetAmount(string name)
        => storage.Amount;

    public int GetMaxAmount(string name)
        => storage.MaxAmount;

    public void AddListenerFindNewStorage(UnityAction<string[]> action)
        => FindNewStorage.AddListener(action);

    public void RemoveListenerFindNewStorage(UnityAction<string[]> action)
        => FindNewStorage.RemoveListener(action);

    public bool IsLocal { get; set; } = true;

    public Transform Destination { get => destination; }
}

[Serializable]
public class Employee
{
    public string name;
    public Sprite fullSprite;
    public Sprite helm;
    public Sprite armorL;
    public Sprite armorR;
}