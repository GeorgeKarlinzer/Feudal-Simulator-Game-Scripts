using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldiersSystem : MonoBehaviour
{
    [SerializeField] private LayerMask humanLayer;
    [SerializeField] private Camera cam;
    [SerializeField] private SoldierInfo menu;
    [SerializeField] private GameObject panel;

    public AllyGroup selectedGroup;
    bool update = false;

    private void Awake()
    {
        enabled = false;
        update = false;
    }

    public void OpenMenu()
    {
        UnselectGroup(selectedGroup);
        menu.gameObject.SetActive(true);
        menu.Show(selectedGroup);
    }

    public void Employ(int i)
    {
        // TODO: Move employing system to arsenal
    }

    public void SelectGroup(AllyGroup group)
    {
        if (group.IsAttack)
            return;

        if (enabled)
        {
            UnselectGroup(group);
            if (group == selectedGroup)
                return;
        }

        panel.SetActive(true);

        selectedGroup = group;
        enabled = true;

        group.SetGroupElipceState(true);
    }

    public void UnselectGroup(AllyGroup group)
    {
        // TODO: Check when condition is true
        if (!enabled || group == null || group != selectedGroup)
            throw new System.Exception("Ошибка проверь"); 

        panel.SetActive(false);

        enabled = false;
        update = false;

        group.SetGroupElipceState(false);

        selectedGroup = null;
    }

    private void Update()
    {
        // Wait for one frame
        if (!update)
        {
            update = true;
            return;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0) && selectedGroup.CanInteract)
#else
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended)
#endif
        {
            if (Cursor.IsOverUIElement())
            {
                UnselectGroup(selectedGroup);
                return;
            }
#if UNITY_EDITOR
            Vector3 touchPos = cam.ScreenToWorldPoint(Input.mousePosition);
#else
            Vector3 touchPos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
            var rayCast = Physics2D.Raycast(touchPos, Vector2.zero, float.PositiveInfinity, humanLayer);
            if (rayCast && rayCast.collider.TryGetComponent<Enemy>(out var enemy))
            {
                selectedGroup.GoAttack(enemy.Group);
                UnselectGroup(selectedGroup);
                return;
            }

            if (MyAstarHandler.IsWalkable(touchPos))
                selectedGroup.GoTo(touchPos);

            UnselectGroup(selectedGroup);
        }
    }
}
