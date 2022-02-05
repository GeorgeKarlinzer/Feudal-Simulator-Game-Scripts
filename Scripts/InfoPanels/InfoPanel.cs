using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static StaticData;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private Image classImage;
    [SerializeField] private Image buildingImage;
    [SerializeField] private GameObject changedPanel;
    [SerializeField] private GameObject sellButton;

    public UnityAction onHideAction;

    private Building target = null;
    public void Show(Building building)
    {
        AutoClosingSystemStatic.SetAction(Hide);

        if (target != null) target.UpdateUI.RemoveAllListeners();
        target = building;
        target.info.panel.GetComponent<BuildingInfo>().SetTarget(target);

        sizeText.text = $"{target.Size.x}x{target.Size.y}";
        healthText.text = $"{target.Health}/{target.MaxHealth}";

        descriptionText.text = target.info.description;
        nameText.text = building.name.Remove(building.name.Length - 11);

        buildingImage.sprite = target.info.sprite;
        classImage.sprite = target.info.classSprite;

        if (changedPanel.transform.childCount > 0)
            Destroy(changedPanel.transform.GetChild(0).gameObject);

        var p = Instantiate(target.info.panel, changedPanel.transform);
        p.GetComponent<BuildingInfo>().InitUI();

        building.UpdateUI.AddListener(p.GetComponent<BuildingInfo>().UpdateUI);

        gameObject.SetActive(true);
    }

    public void Sell()
    {
        target.Sell();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (onHideAction != null)
        {
            onHideAction.Invoke();
            onHideAction = null;
        }
    }

    public void HideSellButton()
    {
        sellButton.SetActive(false);
    }

    public void ShowSellButton()
    {
        sellButton.SetActive(true);
    }
}
