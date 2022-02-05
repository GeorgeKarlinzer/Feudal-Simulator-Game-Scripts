using UnityEngine;

public class ButtonMenu : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    public Animator animator;

    public GameObject resourceTab;
    public GameObject buildMenu;

    public void ButtonMenuClicked()
    {
        IsOpen = !IsOpen;

        animator.SetTrigger(IsOpen ? "Open" : "Close");

        if (!IsOpen)
        {
            resourceTab.SetActive(false);
            buildMenu.SetActive(false);
        }
    }

    public void ButtonResourcesClicked()
    {
        resourceTab.SetActive(!resourceTab.activeSelf);
    }

    public void ButtonBuildMenuClicked()
    {
        resourceTab.SetActive(!buildMenu.activeSelf);
        buildMenu.SetActive(!buildMenu.activeSelf);
    }
}
