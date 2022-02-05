using UnityEngine;

public class Storage
{
    private int amount;
    private int maxAmount;
    public int Amount
    {
        get => amount; 
        set
        {
            if(value > maxAmount || value < 0)
            {
                throw new System.Exception("Не было проверки добляемого значения!");
            }
            ResManager.res[Name].FromStorage(value - amount);
            amount = value;
        }
    }
    public int MaxAmount 
    {
        get => maxAmount;
        private set
        {
            maxAmount = value;
            ResManager.res[Name].MaxAmount += maxAmount;
        }
    }
    public string Name { get; private set; }

    public Storage(GameObject source, int maxAmount, string name)
    {
        Name = name;
        Source = source;
        MaxAmount = maxAmount;
        Amount = 0;
    }

    public GameObject Source { get; private set; }
}
