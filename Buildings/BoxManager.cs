using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private Transform[] boxesGroup = new Transform[3];

    public int MaxRes { get; set; }
    private int[] values = { 0, 0, 0 };

    public void ChangeBoxes(int newValue, int index)
    {
        if (newValue == values[index])
            return;

        var boxesPerRes = boxesGroup[index].childCount / (float)MaxRes;
        var boxNewValue = Mathf.RoundToInt(newValue * boxesPerRes);
        var boxValue = Mathf.RoundToInt(values[index] * boxesPerRes);
        var boxDelta = boxNewValue - boxValue;

        for (var i = 0; Mathf.Abs(i) < Mathf.Abs(boxDelta); i += (int)Mathf.Sign(boxDelta))
            boxesGroup[index].GetChild(boxValue + i - (boxDelta > 0 ? 0 : 1))
                .gameObject.SetActive(boxDelta > 0);

        values[index] = newValue;
    }

    public void ChangeBoxSprites(Sprite sprite, int index)
    {
        foreach (Transform t in boxesGroup[index])
            t.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
