using UnityEngine;

public struct Colors
{
    public static Color BlackUI => new Color(0.196f, 0.196f, 0.196f);
    public static Color RedUI => new Color(1, 0.27f, 0.27f);
    public static Color Red => new Color(0.87f, 0.37f, 0.37f);
    public static Color White => new Color(1, 1, 1);

    public static void ChangeColor(ISelectable selectable, Color color, int deltaSortingLayer)
    {
        foreach (var s in selectable.Graphics)
        {
            Color.RGBToHSV(s.color, out float _, out _, out float V);
            Color.RGBToHSV(color, out float newH, out float newS, out float _);
            s.color = Color.HSVToRGB(newH, newS, V);
            s.sortingOrder += deltaSortingLayer;
        }
    }
}
