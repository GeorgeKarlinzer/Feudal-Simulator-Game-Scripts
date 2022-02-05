using UnityEngine;
using UnityEngine.UI;
using static StaticData;

public class SideMenu : MonoBehaviour
{
    [SerializeField] private BufferBuilding bufferBuilding;
    [SerializeField] private BufferDoor bufferDoor;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite infoSprite;
    [SerializeField] private Sprite sellSprite;
    [SerializeField] private Button showInfoButton;

    private GameObject target;

    public void ShowSideMenu(GameObject target)
    {
        this.target = target;

        Building building = target.GetComponent<Building>();
        if (building)
        {
            SellToInfo();
            MoveButtonStatic.gameObject.SetActive(building.canMove);
        }
        else
            InfoToSell();

        animator.SetTrigger("Open");

        AutoClosingSystemStatic.SetAction(HideSideMenu);

        Colors.ChangeColor(target.GetComponent<ISelectable>(), Colors.Red, 1);
    }

    public void ShowInfo()
    {
        InfoPanelStatic.Show(target.GetComponent<Building>());
    }

    public void Move()
    {
        Cursor.IsBusy = true;
        BufferObject bufferObj = bufferDoor;
        bufferObj.isBeforeBuy = false;
        target.SetActive(false);

        HideSideMenu();

        Building building = target.GetComponent<Building>();
        if (building)
        {
            bufferObj = bufferBuilding;
            bufferObj.isBeforeBuy = false;

            bufferObj.GetComponent<SpriteRenderer>().sprite = building.movingSprite;
            bufferObj.GetComponent<BoxCollider2D>().size = target.GetComponent<BoxCollider2D>().size;
            bufferObj.GetComponent<BufferBuilding>().size = building.Size;
            RotateButtonStatic.gameObject.SetActive(building.canRotate);
        }
        else
        {
            DoorPlace.Show();
            RotateButtonStatic.gameObject.SetActive(false);
        }

        bufferObj.OnMouseUp();

        BuildButtonStatic.onClick.AddListener(() => bufferObj.ChangePosition(target));
        CancelButtonStatic.onClick.AddListener(CancelMooving);
        CancelButtonStatic.onClick.AddListener(bufferObj.Cancel);

        bufferObj.transform.rotation = target.transform.rotation;
        bufferObj.transform.position = target.transform.position;

        UIManagerStatic.HideGUI();
        RedactingGridStatic.ShowGrid();

        bufferObj.gameObject.SetActive(true);
    }

    public void CancelMooving()
    {
        target.SetActive(true);

        BuildButtonStatic.onClick.RemoveAllListeners();
        CancelButtonStatic.onClick.RemoveAllListeners();
        RedactingGridStatic.HideGrid();

        if (target.GetComponent<Building>())
            MapManagerStatic.SetMapFullness(target.GetComponent<Building>().Size, target.transform.position, false);
    }

    public void HideSideMenu()
    {
        if (target == null)
            return;

        animator.SetTrigger("Close");

        Colors.ChangeColor(target.GetComponent<ISelectable>(), Colors.White, -1);
    }

    /// <summary>
    /// Заменяет функционал кнопки "Показать информацию" на кнопку продажи
    /// </summary>
    public void InfoToSell()
    {
        showInfoButton.transform.GetChild(0).GetComponent<Image>().sprite = sellSprite;
        showInfoButton.onClick.RemoveAllListeners();
        showInfoButton.onClick.AddListener(target.GetComponent<Marketable>().Sell);
        showInfoButton.onClick.AddListener(HideSideMenu);

    }

    public void SellToInfo()
    {
        showInfoButton.transform.GetChild(0).GetComponent<Image>().sprite = infoSprite;
        showInfoButton.onClick.RemoveAllListeners();
        showInfoButton.onClick.AddListener(ShowInfo);
        showInfoButton.onClick.AddListener(HideSideMenu);
    }
}
