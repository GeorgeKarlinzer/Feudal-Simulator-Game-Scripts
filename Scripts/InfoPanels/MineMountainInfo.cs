using static StaticData;

public class MineMountainInfo : BuildingInfo
{
    public void Buy()
    {
        if (GetTarget<MineMountain>().BuyMine())
            InfoPanelStatic.Hide();
    }

    private void OnEnable()
    {
        InfoPanelStatic.HideSellButton();
    }

    private void OnDisable()
    {
        InfoPanelStatic.ShowSellButton();   
    }
}
