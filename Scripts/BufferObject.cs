using UnityEngine;
using UnityEngine.EventSystems;
using static StaticData;

public class BufferObject : MonoBehaviour
{
    protected Transform _transform;
    [HideInInspector] public bool isBeforeBuy;

    protected void Awake()
    {
        _transform = gameObject.transform;
    }

    public virtual void Update() { }

    public virtual void TryBuild()
    {
        Cancel();
    }

    public virtual void Cancel()
    {
        Cursor.IsBusy = false;

        enabled = true;
        gameObject.SetActive(false);

        BuildButtonStatic.onClick.RemoveAllListeners();
        CancelButtonStatic.onClick.RemoveAllListeners();

        PopUpStatic.BeforeBuildPanelHide();

        UIManagerStatic.ShowGUI();
        RedactingGridStatic.HideGrid();
    }

    public virtual void ChangePosition(GameObject movedEntity)
    {
        movedEntity.transform.position = transform.position;
        movedEntity.transform.rotation = transform.rotation;
        movedEntity.SetActive(true);
        SideMenuStatic.CancelMooving();
        Cancel();
    }

    public void OnMouseUp()
    {
        EndDrag();
    }

    protected void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            BeginDrag();
    }

    protected void BeginDrag()
    {
        Cursor.IsMoveBuilding = true;
        enabled = true;
        PopUpStatic.BeforeBuildPanelHide();
        PopUpStatic.ResourcePanelHide();
    }

    protected void EndDrag()
    {
        Cursor.IsMoveBuilding = false;
        enabled = false;
        PopUpStatic.BeforeBuildPanelShow();
        if (isBeforeBuy)
            PopUpStatic.ResourcePanelShow();
    }
}
