using UnityEngine;

public class Gate : Building
{
    /// <summary>
    /// Размер без учёта открывающихся ворот
    /// </summary>
    public Vector2Int trueSize;
    public bool needSideWalls;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject gateCollider;
    private bool isOpen = false;

    public void ChangeGateState()
    {
        if (isOpen != gateCollider.activeSelf)
        {
            if (isOpen)
                Close();
            else
                Open();

            isOpen = !isOpen;
        }
    }

    private void Open()
    {
        animator.SetTrigger("Open");
    }

    private void Close()
    {
        animator.SetTrigger("Close");
    }

    public void AddPath()
    {
        gateCollider.SetActive(false);
        MyAstarHandler.UpdateArea(transform.position, Size);
    }

    public void RemovePath()
    {
        gateCollider.SetActive(true);
        MyAstarHandler.UpdateArea(transform.position, Size);
    }
}
