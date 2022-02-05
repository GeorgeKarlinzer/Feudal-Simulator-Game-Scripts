using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class Ally : MonoBehaviour
{
    [SerializeField] private GameObject elipce;

    [SerializeField] private SoldierBase soldier;
    public AllyGroup Group => (AllyGroup)soldier.Group;


    private void OnMouseUp()
    {
        if(Group.CanInteract && !Cursor.IsBusy)
            SoldiersSystemStatic.SelectGroup(Group);
    }

    public void SetElipceState(bool state) =>
        elipce.SetActive(state);
}
