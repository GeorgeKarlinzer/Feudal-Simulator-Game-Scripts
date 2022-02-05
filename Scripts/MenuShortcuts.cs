using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShortcuts : MonoBehaviour
{
    [SerializeField] private BuildMenu buildMenu;
    [SerializeField] private ButtonMenu buttonMenu;
#if UNITY_EDITOR
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            buttonMenu.ButtonBuildMenuClicked();
        }

        if(Input.GetKeyDown(KeyCode.Tab) && buttonMenu.buildMenu.activeInHierarchy)
        {
            buildMenu.SelectGroup((buildMenu.OldIndex + 1) % buildMenu.PanelLength);
        }
    }
#endif
}
