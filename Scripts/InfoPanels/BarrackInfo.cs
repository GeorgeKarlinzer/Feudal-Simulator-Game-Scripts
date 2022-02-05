using TMPro;
using UnityEngine;

public class BarrackInfo : BuildingInfo
{
    private Barrack Target { get => GetTarget<Barrack>(); }

    [SerializeField] private TextMeshProUGUI noWorkText;
    [SerializeField] private TextMeshProUGUI vacanciesText;

    private void OnEnable()
    {
        UpdateUI();
    }

    public override void UpdateUI()
    {
        noWorkText.text = string.Format("{0}/{1}",
            Target.NoWorkHumans.ToString(),
            Target.GetMaxAmount(ResManager.Human));

        vacanciesText.text = string.Format("{0}/{1}",
            Target.BoughtHumans,
            Target.GetMaxAmount(ResManager.Human));
    }

    public override void InitUI() 
    {
        UpdateUI();
    }

    public void Employ()
    {
        Target.Employ();
        UpdateUI();
    }

    public void Dismiss()
    {
        Target.Dismiss();
        UpdateUI();
    }
}