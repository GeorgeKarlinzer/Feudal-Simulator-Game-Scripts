using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StaticData;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private GameObject pricePanel;
    [SerializeField] private GameObject pricePrefab;

    [SerializeField] private GameObject beforeBuildPanel;
    [SerializeField] private GameObject resourcePanel;

    public bool IsResourcePanelActive { get => resourcePanel.activeSelf; }

    public void ResourcePanel(Building building)
    {
        pricePanel.SetActive(true);
        AutoClosingSystemStatic.SetAction(() => pricePanel.SetActive(false));
        Transform content = pricePanel.transform.GetChild(0);

        foreach (Transform t in content)
            Destroy(t.gameObject);

        foreach (Price price in building.prices)
        {
            GameObject priceObject = Instantiate(pricePrefab, content);
            priceObject.transform.GetChild(0).GetComponent<Image>().sprite = ResManager.res[price.name].ResSprite;
            priceObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = price.amount.ToString();
        }
    }

    public void SetResourePanelPosition(RectTransform trans)
    {
        Transform panelTrans = pricePanel.transform.GetChild(0);
        panelTrans.position = new Vector2(trans.position.x, panelTrans.position.y);
    }

    public void BeforeBuildPanelShow()
    {
        beforeBuildPanel.SetActive(true);
    }

    public void ResourcePanelShow()
    {
        resourcePanel.SetActive(true);
    }

    public void BeforeBuildPanelHide()
    {
        beforeBuildPanel.SetActive(false);
    }

    public void ResourcePanelHide()
    {
        resourcePanel.SetActive(false);
    }
}
