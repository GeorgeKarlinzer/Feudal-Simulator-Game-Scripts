using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class Wall : Building
{
    private static List<GameObject> selectedWalls = new List<GameObject>();

    public static void SelectAll(Wall main)
    {
        CancelSelect();

        main.GetComponent<SpriteRenderer>().color = Colors.Red;
        selectedWalls.Add(main.gameObject);

        SelectNext(main.transform.position, new Vector2Int(1, 0));
        SelectNext(main.transform.position, new Vector2Int(0, 1));
        SelectNext(main.transform.position, new Vector2Int(0, -1));
        SelectNext(main.transform.position, new Vector2Int(-1, 0));
    }

    private static void SelectNext(Vector2 pos, Vector2Int step)
    {
        var raycast = Physics2D.RaycastAll(pos + step, Vector2.zero);
        foreach (var r in raycast)
        {
            var item = r.collider.GetComponent<Wall>();
            if (item)
            {
                item.GetComponent<SpriteRenderer>().color = Colors.Red;
                selectedWalls.Add(item.gameObject);
                SelectNext(pos + step, step);
            }
        }
    }

    private bool wasResoursePanelOnScreen;
    public void OpenBuildMenu(Gate gate)
    {
        wasResoursePanelOnScreen = PopUpStatic.IsResourcePanelActive;

        RotateButtonStatic.gameObject.SetActive(false);
        BuildButtonStatic.onClick.AddListener(() => BuildGate(gate));
        CancelButtonStatic.onClick.AddListener(() => CloseBuildMenu(gate));
        InfoPanelStatic.Hide();
        PopUpStatic.BeforeBuildPanelShow();
        UIManagerStatic.HideGUI();
        PopUpStatic.ResourcePanelShow();

        var gateRotation = transform.rotation;
        var size = gateRotation.eulerAngles.z % 180 == 0 ? gate.Size : new Vector2Int(gate.Size.y, gate.Size.x);
        var gatePosition = transform.position + new Vector3(size.x % 2 == 0 ? 0.5f : 0, size.y % 2 == 0 ? 0.5f : 0, 0);
        BuildMenuStatic.ShowBufferBuildingAtPosition(gatePosition, gateRotation, gate.movingSprite);

        Cursor.IsBusy = true;

        foreach (var price in gate.prices)
            ResManager.res[price.name].SetPossibleAmount(price.amount);
    }

    public bool CanBuildGate(Gate gate)
    {
        var gateRotation = transform.rotation;
        var size = gateRotation.eulerAngles.z % 180 == 0 ? gate.Size : new Vector2Int(gate.Size.y, gate.Size.x);
        var trueSize = gateRotation.eulerAngles.z % 180 == 0 ? gate.trueSize : new Vector2Int(gate.trueSize.y, gate.trueSize.x);
        var gatePosition = transform.position + new Vector3(size.x % 2 == 0 ? 0.5f : 0, size.y % 2 == 0 ? 0.5f : 0, 0);

        var wallsToRemove = new List<Wall>();
        var startPos = gatePosition - new Vector3(trueSize.x - 1, trueSize.y - 1, 0) / 2;

        for (var x = 0; x < trueSize.x; x++)
        {
            for (var y = 0; y < trueSize.y; y++)
            {
                var hit = Physics2D.RaycastAll(startPos + new Vector3(x, y, 0), Vector2.zero);
                foreach (var h in hit)
                {
                    Wall wall = h.collider.GetComponent<Wall>();
                    if (wall)
                    {
                        MapManagerStatic.SetMapFullness(wall.Size, wall.transform.position, true);
                        wallsToRemove.Add(wall);
                    }
                }
            }
        }

        if (wallsToRemove.Count != trueSize.x * trueSize.y || !MapManagerStatic.GetMapFullness(size, gatePosition))
        {
            for (var i = 0; i < wallsToRemove.Count; i++)
                MapManagerStatic.SetMapFullness(wallsToRemove[i].Size, wallsToRemove[i].transform.position, false);
            wallsToRemove.Clear();
            return false;
        }

        for (var i = 0; i < wallsToRemove.Count; i++)
                MapManagerStatic.SetMapFullness(wallsToRemove[i].Size, wallsToRemove[i].transform.position, false);

        return true;
    }

    public void BuildGate(Gate gate)
    {
        var gateRotation = transform.rotation;
        var size = gateRotation.eulerAngles.z % 180 == 0 ? gate.Size : new Vector2Int(gate.Size.y, gate.Size.x);
        var trueSize = gateRotation.eulerAngles.z % 180 == 0 ? gate.trueSize : new Vector2Int(gate.trueSize.y, gate.trueSize.x);
        var gatePosition = transform.position + new Vector3(size.x % 2 == 0 ? 0.5f : 0, size.y % 2 == 0 ? 0.5f : 0, 0);

        var wallsToRemove = new List<Wall>();
        var startPos = gatePosition - new Vector3(trueSize.x - 1, trueSize.y - 1, 0) / 2;

        for (var x = 0; x < trueSize.x; x++)
        {
            for (var y = 0; y < trueSize.y; y++)
            {
                var hit = Physics2D.RaycastAll(startPos + new Vector3(x, y, 0), Vector2.zero);
                foreach (var h in hit)
                {
                    Wall wall = h.collider.GetComponent<Wall>();
                    if (wall)
                    {
                        MapManagerStatic.SetMapFullness(wall.Size, wall.transform.position, true);
                        wallsToRemove.Add(wall);
                    }
                }
            }
        }

        if (wallsToRemove.Count != trueSize.x * trueSize.y || !MapManagerStatic.GetMapFullness(size, gatePosition))
        {
            for (var i = 0; i < wallsToRemove.Count; i++)
                MapManagerStatic.SetMapFullness(wallsToRemove[i].Size, wallsToRemove[i].transform.position, false);
            wallsToRemove.Clear();
            return;
        }

        for (var i = 0; i < wallsToRemove.Count; i++)
        {
            if (gate.needSideWalls && (i == 0 || i == wallsToRemove.Count - 1))
                MapManagerStatic.SetMapFullness(wallsToRemove[i].Size, wallsToRemove[i].transform.position, false);
            else
                wallsToRemove[i].Destroy();
        }

        wallsToRemove.Clear();

        Instantiate(gate, gatePosition, gateRotation).Build();
        CloseBuildMenu(gate);
    }

    public void CloseBuildMenu(Gate gate)
    {
        BuildButtonStatic.onClick.RemoveAllListeners();
        CancelButtonStatic.onClick.RemoveAllListeners();
        RotateButtonStatic.gameObject.SetActive(true);
        PopUpStatic.BeforeBuildPanelHide();
        UIManagerStatic.ShowGUI();
        BuildMenuStatic.HideBufferBuilding();

        if (!wasResoursePanelOnScreen)
            PopUpStatic.ResourcePanelHide();

        Cursor.IsBusy = false;

        foreach (var price in gate.prices)
            ResManager.res[price.name].SetNormalAmount();
    }

    public static void CancelSelect()
    {
        foreach (var item in selectedWalls)
            item.GetComponent<SpriteRenderer>().color = Colors.White;

        selectedWalls.Clear();
    }

    public static void SellAll()
    {
        while (selectedWalls.Count > 0)
        {
            selectedWalls[0].GetComponent<Marketable>().Sell();
            selectedWalls.RemoveAt(0);
        }
    }
}
