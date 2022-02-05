using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StaticData;

public class WallInfo : BuildingInfo
{
    [SerializeField] private Button sellAllButton;
    [SerializeField] private TextMeshProUGUI selectAllTMPro;
    [SerializeField] private string selectAllText;
    [SerializeField] private string unselectText;
    [SerializeField] private GameObject[] buyGateButtons;
    [SerializeField] private Gate[] gates;

    private void OnEnable()
    {
        for (int i = 0; i < gates.Length; i++)
            buyGateButtons[i].SetActive(GetTarget<Wall>().CanBuildGate(gates[i]));
    }

    public void OnSelectButtonDown()
    {
        if (selectAllTMPro.text == selectAllText)
            SelectAll();
        else
            Unselect();
    }

    public void TryBuildGate(int index)
    {
        GetTarget<Wall>().OpenBuildMenu(gates[index]);
    }

    private void SelectAll()
    {
        selectAllTMPro.text = unselectText;
        InfoPanelStatic.onHideAction = Unselect;
        Wall.SelectAll(GetTarget<Wall>());
        sellAllButton.gameObject.SetActive(true);
    }

    private void Unselect()
    {
        selectAllTMPro.text = selectAllText;
        InfoPanelStatic.onHideAction = null;
        Wall.CancelSelect();
        sellAllButton.gameObject.SetActive(false);
    }

    public void SellAll()
    {
        Wall.SellAll();
        InfoPanelStatic.Hide();
    }
}
