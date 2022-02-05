using System.Collections.Generic;
using UnityEngine;

public class Marketable : MonoBehaviour
{
    public List<Price> prices = new List<Price>();

    public virtual void Sell()
    {
        foreach (var price in prices)
            ResManager.res[price.name].ToStorage(price.amount / 2);
    }
}
