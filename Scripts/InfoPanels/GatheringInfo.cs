using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GatheringInfo : BuildingInfo
{
    private GatheringBuilding Target { get => GetTarget<GatheringBuilding>(); }

    [SerializeField] private TextMeshProUGUI workerNameText;
    [SerializeField] private TextMeshProUGUI resNameText;
    [SerializeField] private TextMeshProUGUI employedWorkersText;
    [SerializeField] private TextMeshProUGUI amountPerDayText;
    [SerializeField] private TextMeshProUGUI maxAmountText;
    [SerializeField] private TextMeshProUGUI actualAmountText;
    [SerializeField] private Image workerImage;
    [SerializeField] private Image resImage;
    [SerializeField] private Image indicator;

    public void Employ()
    {
        if (Target.EmployedHumans < Target.MaxWorkers)
        {
            Target.Employ();
            employedWorkersText.text = $"{Target.EmployedHumans} / {Target.MaxWorkers}";
            amountPerDayText.text = $"{Target.secondsPerResource * 24 * 3600 * Target.EmployedHumans} / Day";
        }
    }

    public void Dismiss()
    {
        Target.Dismiss();
        employedWorkersText.text = $"{Target.EmployedHumans} / {Target.MaxWorkers}"; 
        amountPerDayText.text = $"{Target.secondsPerResource * 24 * 3600 * Target.EmployedHumans} / Day";
    }

    public override void InitUI()
    {
        workerNameText.text = Target.employee.name;
        string resName = Target.ResName;
        resNameText.text = resName;
        amountPerDayText.text = $"{Target.secondsPerResource * 24 * 3600 * Target.EmployedHumans} / Day";
        maxAmountText.text = Target.GetMaxAmount(resName).ToString();
        actualAmountText.text = $"{Target.GetAmount(resName)}";
        workerImage.sprite = Target.employee.fullSprite;
        resImage.sprite = ResManager.res[resName].ResSprite;
        employedWorkersText.text = $"{Target.EmployedHumans} / {Target.MaxWorkers}";
        indicator.fillAmount = Target.GetAmount(resName) / (float)Target.GetMaxAmount(resName);
    }

    public override void UpdateUI()
    {
        string resName = resNameText.text;
        float amount = Target.GetAmount(resName);
        float maxAmount = Target.GetMaxAmount(resName);

        actualAmountText.text = amount.ToString();
        indicator.fillAmount = amount / maxAmount;
    }
}
