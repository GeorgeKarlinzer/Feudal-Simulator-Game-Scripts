public class ArsenalInfo : BuildingInfo
{
    public void Employ(int i)
        => GetTarget<Arsenal>().Employ(i);
}
