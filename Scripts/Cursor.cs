using UnityEngine;
using UnityEngine.EventSystems;

public static class Cursor
{
    private static bool isBusy;
    
    public static bool IsMoveBuilding { get; set; }

    public static bool IsCameraMoving { get; set; }

    public static bool IsBusy { get => isBusy || IsOverUIElement(); set => isBusy = value; }

    public static bool IsOverUIElement() => EventSystem.current.IsPointerOverGameObject() || CheckTachUI();

    private static bool CheckTachUI()
    {
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                // ui touched
                return true;
            }
        }
        return false;
    }
}
