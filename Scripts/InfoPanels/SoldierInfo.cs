using UnityEngine;
using UnityEngine.UI;
using static StaticData;

public class SoldierInfo : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image[] images;
    [SerializeField] private GroupBase group;

    public void Show(GroupBase group)
    {
        AutoClosingSystemStatic.SetAction(() => gameObject.SetActive(false));
        this.group = group;
        UpdateUI();
    }

    public void UpdateUI()
    {
        int c = group.soldiers.Count;
        for (int i = 0; i < images.Length; i++)
            images[i].gameObject.SetActive(c > i);
    }

    public void Remove(int i)
    {
        Destroy(group.soldiers[i]);
        UpdateUI();
    }
}
