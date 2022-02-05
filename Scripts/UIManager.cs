using UnityEngine;

public class UIManager : MonoBehaviour
{
    public const int zPos = -10000;

    [SerializeField] private GameObject[] hiddenObjects;
    [SerializeField] private GameObject[] fakeHiddenObjects;

    public void HideGUI()
    {
        foreach(var g in hiddenObjects)
            g.SetActive(false);

        foreach(var g in fakeHiddenObjects)
                g.transform.localPosition = new Vector3(g.transform.localPosition.x, g.transform.localPosition.y, zPos);
    }

    public void ShowGUI()
    {
        foreach (var g in hiddenObjects)
            g.SetActive(true);

        foreach (var g in fakeHiddenObjects)
            g.transform.localPosition = new Vector3(g.transform.localPosition.x, g.transform.localPosition.y, 0);
    }
}
