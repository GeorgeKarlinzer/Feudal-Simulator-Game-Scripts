using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StaticData;

public class BuildMenu : MonoBehaviour
{
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;
    Vector3 selectedScale = new Vector3(1.1f, 1.1f, 1);
    Vector3 normalScale = new Vector3(1, 1, 1);

    [SerializeField] private GameObject[] icons;
    [SerializeField] private GameObject[] panels;

    [SerializeField] private BufferTower bufferTower;
    [SerializeField] private BufferBuilding bufferBuilding;
    [SerializeField] private BufferDoor bufferDoor;

    [SerializeField] private TextMeshProUGUI freeDoorsText;

    private int freeDoors;
    public int FreeDoors
    {
        get => freeDoors;
        set
        {
            freeDoorsText.text = value.ToString();
            freeDoors = value;
        }
    }

    public int OldIndex { get; private set; } = 0;
    public int PanelLength => panels.Length;


    public void SelectGroup(int newIndex)
    {
        if (newIndex == OldIndex)
            return;

        icons[newIndex].GetComponent<Image>().sprite = selectedSprite;
        icons[OldIndex].GetComponent<Image>().sprite = normalSprite;
        icons[newIndex].transform.localScale = selectedScale;
        icons[OldIndex].transform.localScale = normalScale;

        panels[newIndex].SetActive(true);
        panels[OldIndex].SetActive(false);

        OldIndex = newIndex;
    }

    public void ShowBufferBuildingAtPosition(Vector3 position, Quaternion rotation, Sprite sprite)
    {
        bufferBuilding.GetComponent<SpriteRenderer>().sprite = sprite;
        bufferBuilding.transform.position = position;
        bufferBuilding.transform.rotation = rotation;
        bufferBuilding.enabled = false;
        bufferBuilding.GetComponent<Collider2D>().enabled = false;
        bufferBuilding.gameObject.SetActive(true);
    }

    public void HideBufferBuilding()
    {
        bufferBuilding.gameObject.SetActive(false);
        bufferBuilding.GetComponent<Collider2D>().enabled = true;
        bufferBuilding.enabled = true;
    }

    public void TowerBeginDrag(Building tower)
    {
        BeginDrag(tower, bufferTower);
    }

    public void TowerEndDrag(Marketable tower)
    {
        EndDrag(tower, bufferTower);
    }
    
    private void BeginDrag(Building building, BufferBuilding buffer)
    {
        if (!Price.CanBuy(building.prices.ToArray()))
            return;

        Cursor.IsMoveBuilding = true;
        Cursor.IsBusy = true;
        buffer.isBeforeBuy = true;

        foreach (var price in building.prices)
            ResManager.res[price.name].SetPossibleAmount(price.amount);

        buffer.GetComponent<SpriteRenderer>().sprite = building.movingSprite;
        buffer.GetComponent<BoxCollider2D>().size = building.GetComponent<BoxCollider2D>().size;
        buffer.transform.rotation = Quaternion.Euler(Vector3.zero);

        buffer.gameObject.SetActive(true);
        buffer.Update();

        buffer.size = building.Size;
        buffer.buildingPrefab = building;

        BuildButtonStatic.onClick.AddListener(buffer.TryBuild);
        CancelButtonStatic.onClick.AddListener(buffer.Cancel);
        RotateButtonStatic.gameObject.SetActive(building.canRotate);

        RedactingGridStatic.ShowGrid();
        UIManagerStatic.HideGUI();
    }

    private void EndDrag(Marketable m, BufferBuilding buffer)
    {
        if (!Price.CanBuy(m.prices.ToArray()))
            return;

        buffer.OnMouseUp();
    }

    public void BuildingBeginDrag(GameObject building)
    {
        BeginDrag(building.GetComponent<Building>(), bufferBuilding);
    }

    public void BuildingEndDrag(Marketable m)
    {
        EndDrag(m, bufferBuilding);
    }

    public void DoorBeginDrag(GameObject door)
    {
        if (DoorPlace.TreeCount == 0)
            return;

        Marketable m = door.GetComponent<Marketable>();

        if (freeDoors == 0)
        {
            if (!Price.CanBuy(m.prices.ToArray()))
                return;

            foreach (var price in m.prices)
                ResManager.res[price.name].SetPossibleAmount(price.amount);
        }

        Cursor.IsMoveBuilding = true;
        Cursor.IsBusy = true;

        bufferDoor.isBeforeBuy = true;
        bufferDoor.gameObject.SetActive(true);
        bufferDoor.Update();

        BuildButtonStatic.onClick.AddListener(bufferDoor.TryBuild);
        RotateButtonStatic.gameObject.SetActive(false);
        CancelButtonStatic.onClick.AddListener(bufferDoor.Cancel);

        RedactingGridStatic.ShowGrid();
        UIManagerStatic.HideGUI();

        DoorPlace.Show();
    }

    public void DoorEndDrag(Marketable m)
    {
        if (DoorPlace.TreeCount == 0)
            return;

        if (!Price.CanBuy(m.prices.ToArray()) && freeDoors == 0)
            return;

        bufferDoor.GetComponent<BufferDoor>().OnMouseUp();
    }
}