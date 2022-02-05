using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarehouseInfo : BuildingInfo
{
    private Warehouse Target { get => GetTarget<Warehouse>(); }
    private int currentIndex;

    [SerializeField] private GameObject selectMenu;

    [SerializeField] private TextMeshProUGUI[] amountTexts;
    [SerializeField] private TextMeshProUGUI[] resNameTexts;
    [SerializeField] private Image[] indicators;
    [SerializeField] private Image[] resImages;
    [SerializeField] private Sprite unknownSprite;

    public override void UpdateUI()
    {
        for (int i = 0; i < 3; i++)
        {
            amountTexts[i].text = $"{Target.GetAmount(i)}/{Target.MaxAmount}";
            indicators[i].fillAmount = Target.GetAmount(i) / (float)Target.MaxAmount;
        }
    }

    public void OpenSelectMenu(int index)
    {
        currentIndex = index;
        selectMenu.SetActive(true);
    }

    public void SelectRes(string resName)
    {
        if (Target.Select(resName, currentIndex))
        {
            resImages[currentIndex].sprite = ResManager.res[resName].ResSprite;
            resNameTexts[currentIndex].text = resName;
        }

        selectMenu.SetActive(false);
    }

    public void DeleteRes()
    {
        if (resImages[currentIndex].sprite != unknownSprite)
        {
            resImages[currentIndex].sprite = unknownSprite;
            resNameTexts[currentIndex].text = "";
            Target.Delete(currentIndex);
        }
        selectMenu.SetActive(false);
        UpdateUI();
    }

    public override void InitUI()
    {
        for (int i = 0; i < 3; i++)
        {
            string resName = Target.GetResName(i);
            if (resName != "")
                resImages[i].sprite = ResManager.res[resName].ResSprite;
            resNameTexts[i].text = resName;
            UpdateUI();
        }
    }
}
