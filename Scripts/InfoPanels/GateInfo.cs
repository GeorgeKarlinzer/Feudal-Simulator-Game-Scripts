public class GateInfo : BuildingInfo
{
    public void ChangeGateState()
    {
        GetTarget<Gate>().ChangeGateState();
    }
}
