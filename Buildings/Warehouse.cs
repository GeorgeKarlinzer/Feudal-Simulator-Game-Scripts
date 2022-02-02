using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Warehouse : Building, IStorage
{
    [SerializeField] private BoxManager boxManager;
    [SerializeField] private Transform destination;

    private List<Storage> storage = new List<Storage>();
    [SerializeField] private int maxAmount;
    public int MaxAmount { get => maxAmount; }

    /// <summary>
    /// Вызывается когда хранилище уже не может принимать ресурсы и все рабочие 
    /// направляющиеся в это хранилище должны найти другое хранилище. Принимает массив строк
    /// который указывает на то, какие ресурсы уже не могут приниматься
    /// </summary>
    public StringArrEvent FindNewStorage = new StringArrEvent();

    private void Start()
    {
        boxManager.MaxRes = maxAmount;
        storage.Add(null);
        storage.Add(null);
        storage.Add(null);
        //Стартовый объект на сцене
        if (name == "Warehouse")
        {
            Select(ResManager.Gold, 0);
            AddAmount(ResManager.Gold, maxAmount);
        }
    }

    protected override void OnEnable()
    {
        if (name != "Warehouse")
        {
            base.OnEnable();
            IsLocal = false;
            foreach (var s in storage)
                if (s != null)
                    HumanWorkGlobal.CheckWaiters(s.Name, this);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        IsLocal = true;
    }

    private void OnDestroy()
    {
        FindNewStorage.Invoke(new string[] { GetResName(0), GetResName(1), GetResName(2) });
        for (var i = 0; i < storage.Count; i++)
            if (storage[i] != null)
                ResManager.res[storage[i].Name].RemoveStorage(this);
    }


    public bool Select(string resName, int index)
    {
        if (storage[index] != null)
            if (storage[index].Amount == 0)
            {
                string oldResName = storage[index].Name;
                ResManager.res[oldResName].RemoveStorage(this);
                FindNewStorage.Invoke(new string[] { oldResName });
            }
            else
                return false;

        storage[index] = new Storage(gameObject, maxAmount, resName);
        ResManager.res[resName].AddStorage(this);
        boxManager.ChangeBoxSprites(ResManager.res[resName].BoxSprite, index);
        HumanWorkGlobal.CheckWaiters(resName, this);
        return true;
    }

    public void Delete(int index)
    {
        var oldResName = storage[index].Name;
        boxManager.ChangeBoxes(0, index);
        ResManager.res[oldResName].RemoveStorage(this);
        storage[index] = null;
        FindNewStorage.Invoke(new string[] { oldResName });
    }

    public int GetStorageCount() => storage.Count;

    public int GetAmount(int i) => storage[i] != null ? storage[i].Amount : 0;

    public string GetResName(int i) => storage[i] != null ? storage[i].Name : "";

    // Реализация интерфейса IStorage
    public void AddAmount(string name, int value)
    {
        var isFull = true;
        var isSub = value < 0;

        for (var i = 0; i < storage.Count; i++)
            if (storage[i] != null && storage[i].Name == name)
            {
                var buff = -1;
                if (value > 0)
                    buff = Mathf.Clamp(value, 0, storage[i].MaxAmount - storage[i].Amount);
                else
                    buff = Mathf.Clamp(value, -storage[i].Amount, 0);

                storage[i].Amount += buff;
                value -= buff;

                isFull = storage[i].Amount == storage[i].MaxAmount && isFull;
                boxManager.ChangeBoxes(storage[i].Amount, i);
            }

        UpdateUI.Invoke();
        if (isSub) HumanWorkGlobal.CheckWaiters(name, this);
        if (isFull) FindNewStorage.Invoke(new string[] { name });
    }

    public int GetAmount(string name)
    {
        var amount = 0;

        foreach (var s in storage)
            if (s != null && s.Name == name)
                amount += s.Amount;

        return amount;
    }

    public int GetMaxAmount(string name)
    {
        var maxAmount = 0;

        foreach (var s in storage)
            if (s != null && s.Name == name)
                maxAmount += s.MaxAmount;

        return maxAmount;
    }

    public void AddListenerFindNewStorage(UnityAction<string[]> action)
        => FindNewStorage.AddListener(action);

    public void RemoveListenerFindNewStorage(UnityAction<string[]> action)
        => FindNewStorage.RemoveListener(action);

    public bool IsLocal { get; set; } = false;

    public Transform Destination { get => destination; }
}