using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Barrack : Building, IStorage, IEmployable
{
    [SerializeField] private Transform destination;
    [SerializeField] private RuntimeAnimatorController workerController;
    [SerializeField] private Worker workerPrefab;

    private Storage storage;
    private readonly List<Worker> humans = new List<Worker>(0);

    [SerializeField] private int maxHumans;
    public Price humanPrice = new Price(ResManager.Gold, 0);
    /// <summary>
    /// Количество купленных человек
    /// </summary>
    public int BoughtHumans { get => humans.Count; }
    /// <summary>
    /// Количество не работающих человек
    /// </summary>
    public int NoWorkHumans { get => storage.Amount; }
    public int MaxWorkers { get => maxHumans; }

    private StringArrEvent findNewStorage = new StringArrEvent();

    public void Employ()
    {
        if (BoughtHumans < maxHumans && Price.CanBuy(humanPrice))
        {
            storage.Amount++;
            var human = Instantiate(workerPrefab, destination.position, Quaternion.identity);
            humans.Add(human);
            Price.Spend(humanPrice);
            human.SetController(workerController);
            GetComponent<BarrackWork>().SetNewActivity(human);
            human.barrack = this;
        }
    }

    public void Dismiss()
    {
        if(BoughtHumans > 0)
        {
            var h = humans[humans.Count - 1];
            if (h.workPlace != null)
                h.workPlace.Dismiss(h, true);
            else
                h.StopActivity();
            storage.Amount--;
            humans.Remove(h);
            Destroy(h.gameObject);
        }
    }

    public void GoBack(Worker worker, bool instantly)
    {
        storage.Amount++;
        worker.SetController(workerController);
        GetComponent<BarrackWork>().SetNewActivity(worker);
        if (instantly) worker.transform.position = destination.position;
    }

    public bool Send(Transform target, out Worker human)
    {
        human = null;

        for (var i = 0; i < humans.Count; i++)
        {
            human = humans[i];
            if (human.workPlace == null && MyAstarHandler.IsPathPossible(human.transform, target))
            {
                storage.Amount--;
                return true;
            }
        }

        return false;
    }

    private void Start()
    {
        storage = new Storage(gameObject, maxHumans, ResManager.Human);
        ResManager.res[ResManager.Human].AddStorage(this);
    }

    public override void Destroy()
    {
        while (humans.Count > 0)
        {
            var human = humans[0];
            humans.RemoveAt(0);
            Destroy(human.gameObject);
        }
        ResManager.res[ResManager.Human].RemoveStorage(this);
        base.Destroy();
    }

    public void AddAmount(string name, int value) => storage.Amount += value;

    public int GetAmount(string name) => storage.Amount;

    public int GetMaxAmount(string name) => storage.MaxAmount;

    public void AddListenerFindNewStorage(UnityAction<string[]> action) => findNewStorage.AddListener(action);

    public void RemoveListenerFindNewStorage(UnityAction<string[]> action) => findNewStorage.RemoveListener(action);

    public bool IsLocal { get; set; } = false;

    public Transform Destination { get => destination; }
}
