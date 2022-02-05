[System.Serializable]
public struct Price
{ 
    public Price(string name, int amount)
    {
        this.name = name;
        this.amount = amount;
    }
    public string name;
    public int amount;

    public static bool CanBuy(params Price[] prices)
    {
        foreach (Price p in prices)
            if (p.amount > ResManager.res[p.name].Amount)
                return false;

        return true;
    }

    public static void Spend(params Price[] prices)
    {
        foreach (Price p in prices)
            ResManager.res[p.name].ToStorage(-p.amount);
    }
}
